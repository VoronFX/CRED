using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CRED.Data;
using CRED2.Model;
using LibGit2Sharp;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using Branch = CRED2.Model.Branch;
using Commit = CRED2.Model.Commit;
using GitCommit = LibGit2Sharp.Commit;

namespace CRED2.GitRepository
{
	internal static class CsvKeyValueProvider
	{
		public static bool TryParseCsvLine(string line, out KeyValuePair<string, string> result)
		{
			var match = Regex.Match(line, @"^\s*(?<key>.+?),\s*[""]{3}(?<value>.*)[""]{3}");
			if (!match.Success)
				return false;
			result = new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value);
			return true;
		}
	}

	internal static class GitRepositoryServiceExtensionMethods
	{
		public static IEnumerable<TreeEntry> Flatten(this Tree tree)
			=> tree.SelectMany(x =>
				x.TargetType != TreeEntryTargetType.Tree ? Enumerable.Repeat(x, 1) : ((Tree)x.Target).Flatten());

		public static IEnumerable<string> ReadLines(this Stream stream)
		{
			return ReadLines(() => stream);
		}

		public static IEnumerable<string> ReadLines(Func<Stream> streamProvider)
		{
			using (var stream = streamProvider())
			using (var reader = new StreamReader(stream))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}
	}

	public sealed partial class GitBridgeService : IDisposable
	{
		private RepositoryDispatcher Dispatcher { get; } = new RepositoryDispatcher();

		private string ValuesCacheKey(string commitHash) => $"{commitHash}Values";

		private Dictionary<string, DateTime> BranchNextUpdate { get; } = new Dictionary<string, DateTime>();
		private HistoryRepository Database { get; }
		private IMemoryCache MemoryCache { get; }
		private ILogger<GitBridgeService> Logger { get; }
		private Repository Repository { get; }

		public GitBridgeService(HistoryRepository database, IConfiguration configuration,
			IMemoryCache memoryCache, ILogger<GitBridgeService> logger)
		{
			Database = database;
			MemoryCache = memoryCache;
			Logger = logger;

			var repoPath = configuration.GetConnectionString("GitRepository");
			if (!Repository.IsValid(repoPath))
			{
				Logger.LogWarning("Repository at path {RepositoryPath} is invalid. Removing...", repoPath);
				Directory.Delete(repoPath, true);
				Logger.LogInformation("Initializing new repository at {RepositoryPath}...", repoPath);
				Repository.Init(repoPath);
			}
			Repository = new Repository(repoPath);

			var branches = Database.Fetch<Branch>(x => x.GitBranch);
			foreach (var branch in branches)
			{
				string refSpec = $"+{branch.GitRemoteRef}:refs/remotes/{branch.Name}/{branch.Name}";

				Remote remote = Repository.Network.Remotes.FirstOrDefault(x => x.Name == branch.Name);

				if (remote != null &&
					(remote.FetchRefSpecs.Count() != 1
					 || remote.FetchRefSpecs.First().Specification != refSpec
					 || remote.Url != branch.GitRemoteUrl))
				{
					Logger.LogWarning("Remote configuration mismatch branch {BranchName}. Removing...", branch.Name);
					Repository.Network.Remotes.Remove(branch.Name);
					remote = null;
				}

				if (remote == null)
				{
					Logger.LogInformation("Adding remote for branch {BranchName}", branch.Name);
					Repository.Network.Remotes.Add(branch.Name, branch.GitRemoteUrl, refSpec);
				}
			}

			foreach (var remote in Repository.Network.Remotes
				.Select(x => x.Name)
				.Except(branches.Select(x => x.Name)))
			{
				Logger.LogWarning("Remote {RemoteName} has no matching branch. Removing...", remote);
				Repository.Network.Remotes.Remove(remote);
			}

			foreach (var branch in branches.Where(x => !x.Broken))
				BranchNextUpdate.Add(branch.Name, DateTime.UtcNow);

			Dispatcher.InvokeAsync(AutoUpdate);
		}

		private async Task AutoUpdate()
		{
			foreach (var branch in BranchNextUpdate
				.Where(x => x.Value > DateTime.UtcNow))
			{
				Logger.LogInformation("Initiating auto-update of branch {BranchName}", branch.Key);
				await UpdateBranch(branch.Key);
			}

			var nextCheckTime = BranchNextUpdate
				.Aggregate(DateTime.MaxValue, (time, pair) => pair.Value > time ? pair.Value : time);

			await Task.Delay((nextCheckTime - DateTime.UtcNow).Duration());
			var dummy = Dispatcher.InvokeAsync(AutoUpdate);
		}

		public async Task SetupBranch(string name, LibGit2Sharp.Branch newConfig)
		{

			//branch.Tip


		}

		public async Task UpdateBranch(string name)
		{
			var branch = Database.SingleById<Branch>(name);
			Repository.Network.Fetch(branch.Name,
			Repository.Network.Remotes[branch.Name].FetchRefSpecs.Select(x => x.Specification),
			new FetchOptions
			{
				OnProgress = output =>
				{
					Logger.LogDebug(output);
					return true;
				},
				CredentialsProvider = (url, usernameFromUrl, types)
				=> new UsernamePasswordCredentials
				{
					Username = branch.GitUsername,
					Password = branch.GitPassword
				}
			});

			var commit = await Database.GetCommit(branch.CommitId);
			if (Repository.Branches[branch.Name].Tip.Sha != commit.Hash)
			{
				Logger.LogInformation(
					"Updating branch {BranchName} from {LastSyncedCommitHash} to {NewCommitHash}", branch.Name,
					commit.Hash, Repository.Branches[branch.Name].Tip.Sha);

				if (Repository.Branches[branch.Name].Commits.All(x => x.Sha != commit.Hash))
					Logger.LogWarning("Non fast-forward update of branch {BranchName}", branch.Name);

				try
				{
					using (var transaction = new Transaction(Database))
					{
						transaction.BeforeRollback += (sender, items) =>
						{
							foreach (var rollbackItem in items)
							{
								//TODO: Clear cache from failed items.
							}
						};
						var newCommit = await EnsureChangesCalculated(Repository.Branches[branch.Name].Tip, transaction);
						transaction.Commit();
						branch.CommitId = newCommit.Id;
						BranchNextUpdate[branch.Name] = DateTime.UtcNow + TimeSpan.FromMinutes(1);
						Logger.LogInformation("Update of branch {BranchName} complete", branch.Name);
					}
				}
				catch (Exception e)
				{
					Logger.LogError(e, "Update of branch {BranchName} failed", branch.Name);
					branch.Broken = true;
				}
				Database.Update(branch);
			}
			else
			{
				Logger.LogInformation("Branch {BranchName} is up to date", branch.Name);
			}
		}

		public async Task MergeBranch(IEnumerable<Change> changes, LibGit2Sharp.Branch branch)
		{
			//branch.Tip


		}

		private Task<ImmutableHashSet<Tuple<string, string, string>>> ExtractStrings(GitCommit gitCommit)
		{
			return MemoryCache.GetOrCreateAsync(
				ValuesCacheKey(gitCommit.Sha), entry =>
				{
					entry.Priority = CacheItemPriority.Low;
					entry.SlidingExpiration = TimeSpan.FromMinutes(1);
					return Task.Run(() => gitCommit.Tree
						.Flatten()
						.AsParallel()
						.Where(x => x.TargetType == TreeEntryTargetType.Blob
									&& x.Mode == Mode.NonExecutableFile
									&& string.Equals(Path.GetExtension(x.Name), ".csv", StringComparison.OrdinalIgnoreCase))
						.SelectMany(file =>
						{
							var dictionary = new Dictionary<string, string>();

							foreach (var line in ((Blob)file.Target)
								.GetContentStream()
								.ReadLines()
								.Where(x => !string.IsNullOrWhiteSpace(x)))
							{
								if (!CsvKeyValueProvider.TryParseCsvLine(line, out var result))
								{
									Logger.LogWarning("Error parsing line text {LineText} in file {Path} in {CommitHash}", file.Path, line, gitCommit.Sha);
									continue;
								}
								if (dictionary.ContainsKey(result.Key))
								{
									Logger.LogWarning("Duplicate key {Key} in line text {LineText} in file {Path} in {CommitHash}", result.Key, file.Path, line, gitCommit.Sha);
									continue;
								}
								dictionary.Add(result.Key, result.Value);
							}

							return dictionary
							.Select(pair => new Tuple<string, string, string>(file.Path, pair.Key, pair.Value))
							.ToImmutableHashSet();
						})
						.ToImmutableHashSet());
				});
		}

		private Key FindOrInsertKey(string path, string keyParts, Transaction transaction)
		{
			var key = Database.FirstOrDefault<Key>(x2 => x2.Path == path && x2.KeyParts == keyParts);
			if (key == null)
			{
				key = new Key
				{
					Id = Database.AutoIdGenerator.GenerateNewId<Key>(),
					Path = path,
					KeyParts = keyParts
				};
				transaction.AddRollbackRemove<Key>(key.Id);
				Database.Insert(key.Id);
			}
			return key;
		}

		private async Task<Commit> EnsureChangesCalculated(GitCommit gitCommit, Transaction transaction)
		{
			var commit = Database.FirstOrDefault<Commit>(x => x.Hash == gitCommit.Sha);
			if (commit != null)
			{
				// Already calculated
				return commit;
			}

			var gitParents = gitCommit.Parents.ToImmutableArray();

			foreach (var parent in gitParents)
				await EnsureChangesCalculated(parent, transaction);

			var parents = gitParents
				.Select(x => Database.First<Commit>(x2 => x2.Hash == x.Sha))
				.ToImmutableArray();

			commit = new Commit
			{
				Id = Database.AutoIdGenerator.GenerateNewId<Commit>(),
				Hash = gitCommit.Sha,
				Author = gitCommit.Author,
				Committer = gitCommit.Committer,
				Message = gitCommit.Message,
				Parents = parents.Select(x => x.Id).ToArray()
			};

			var changes = new HashSet<Change>(Change.KeyIdComparer);
			var newValues = await ExtractStrings(gitCommit);

			void AddChange(long keyId, string value)
				=> changes.Add(new Change
				{
					Id = Database.AutoIdGenerator.GenerateNewId<Change>(),
					CommitId = commit.Id,
					KeyId = keyId,
					Value = value
				});

			if (parents.IsEmpty)
			{
				foreach (var extracted in await ExtractStrings(gitCommit))
				{
					var key = FindOrInsertKey(extracted.Item1, extracted.Item2, transaction);
					AddChange(key.Id, extracted.Item3);
				}
			}
			else if (parents.Length == 1)
			{
				var parentValues = await ExtractStrings(gitParents.Single());
				var keys = parentValues.Concat(newValues)
					.Select(x => FindOrInsertKey(x.Item1, x.Item2, transaction))
					.ToImmutableHashSet();

				foreach (var key in keys)
				{
					var newValue = newValues.FirstOrDefault(x => x.Item1 == key.Path && x.Item2 == key.KeyParts);
					if (newValue != null && parentValues.Contains(newValue))
						continue;

					AddChange(key.Id, newValue?.Item3);
				}
			}
			else
			{
				var commonBase = Repository.ObjectDatabase
					.FindMergeBase(gitParents,
						parents.Length == 2 ? MergeBaseFindingStrategy.Standard : MergeBaseFindingStrategy.Octopus);

				var parentHistories = new List<ImmutableArray<Change>>();
				foreach (var parent in parents)
					parentHistories.Add(await Database.GetChangesHistory(parent));

				var commonBaseValues = new List<Tuple<string, string, string>>();

				if (commonBase != null)
				{
					commonBaseValues.AddRange(await ExtractStrings(commonBase));
					var commonBaseCommit = Database.First<Commit>(x2 => x2.Hash == commonBase.Sha);

					for (var i = 0; i < parentHistories.Count; i++)
					{
						parentHistories[i] = parentHistories[i]
							.TakeWhile(x => x.CommitId != commonBaseCommit.Id)
							.ToImmutableArray();
					}
				}

				var keys = parentHistories
					.SelectMany(x => x)
					.Select(x => Database.SingleById<Key>(x.KeyId))
					.Concat(newValues.Concat(commonBaseValues)
						.Select(x => FindOrInsertKey(x.Item1, x.Item2, transaction)))
					.ToImmutableHashSet();

				foreach (var key in keys)
				{
					var newValue = newValues.FirstOrDefault(x => x.Item1 == key.Path && x.Item2 == key.KeyParts)?.Item3;

					if (parentHistories
							.Select(x => x.LastOrDefault(x2 => x2.KeyId == key.Id))
							.Where(x => x != null)
							.Any(x => x.Value != newValue)
						|| commonBaseValues
							.Any(x => x.Item1 == key.Path && x.Item2 == key.KeyParts && x.Item3 != newValue))
					{
						AddChange(key.Id, newValue);
					}
				}
			}

			transaction.AddRollbackRemove<Change>(changes.Select(x => (BsonValue)x.Id));
			Database.Insert(changes);
			transaction.AddRollbackRemove<Commit>(commit.Id);
			Database.Insert(commit);

			if (!newValues.SetEquals(
				(await Database.GetAggregatedChanges(commit)).Select(x =>
				{
					var key = Database.SingleById<Key>(x.KeyId);
					return new Tuple<string, string, string>(key.Path, key.KeyParts, x.Value);
				})))
				throw new Exception("Extracted changes mismatch with commit values");

			return commit;
		}

		private bool disposed;

		public void Dispose()
		{
			if (disposed)
				return;
			disposed = true;
			Dispatcher.Dispose();
			Repository.Dispose();
		}
	}
}