using System;
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

	public sealed partial class GitBridge : IDisposable
	{
		private RepositoryDispatcher Dispatcher { get; } = new RepositoryDispatcher();

		private string ValuesCacheKey(string commitHash) => $"{commitHash}Values";

		private Dictionary<string, DateTime> BranchNextUpdate { get; } = new Dictionary<string, DateTime>();
		private HistoryRepository HistoryDb { get; }
		private IMemoryCache MemoryCache { get; }
		private ILogger<GitBridge> Logger { get; }
		private Repository Repository { get; }

		public GitBridge(HistoryRepository historyDb, IHostingEnvironment environment,
			IMemoryCache memoryCache, ILogger<GitBridge> logger)
		{
			HistoryDb = historyDb;
			MemoryCache = memoryCache;
			Logger = logger;

			var repoPath = Path.Combine(environment.ContentRootPath, "bin", "Repository");
			if (!Repository.IsValid(repoPath))
			{
				Logger.LogWarning("Repository at path {RepositoryPath} is invalid. Removing...", repoPath);
				Directory.Delete(repoPath, true);
				Logger.LogInformation("Initializing new repository at {RepositoryPath}...", repoPath);
				Repository.Init(repoPath);
			}
			Repository = new Repository(repoPath);

			var branches = HistoryDb.Fetch<Branch>(x => x.GitBranch);
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
			Dispatcher.InvokeAsync(AutoUpdate);
		}

		public async Task SetupBranch(string name, LibGit2Sharp.Branch newConfig)
		{

			//branch.Tip


		}

		public async Task UpdateBranch(string name)
		{
			using (var db = HistoryRepositoryFactory())
			{
				var branch = db.SingleById<Branch>(name);
				Repository.Network.Fetch(branch.Name,
				Repository.Network.Remotes[branch.Name].FetchRefSpecs.Select(x => x.Specification),
				new FetchOptions
				{
					OnProgress = OnProgress,
					CredentialsProvider = (url, usernameFromUrl, types)
					=> new UsernamePasswordCredentials
					{
						Username = branch.GitUsername,
						Password = branch.GitPassword
					}
				});

				var commit = await db.GetCommit(branch.CommitId);
				if (Repository.Branches[branch.Name].Tip.Sha != commit.Hash)
				{
					Logger.LogInformation(
						"Updating branch {BranchName} from {LastSyncedCommitHash} to {NewCommitHash}", branch.Name,
						commit.Hash, Repository.Branches[branch.Name].Tip.Sha);

					if (Repository.Branches[branch.Name].Commits.All(x => x.Sha != commit.Hash))
						Logger.LogWarning("Non fast-forward update of branch {BranchName}", branch.Name);

					try
					{
						var newCommit = await EnsureChangesCalculated(Repository.Branches[branch.Name].Tip);
						branch.CommitId = newCommit.Id;
						BranchNextUpdate[branch.Name] = DateTime.UtcNow + TimeSpan.FromMinutes(1);
						Logger.LogInformation("Update of branch {BranchName} complete", branch.Name);
					}
					catch (Exception e)
					{
						Logger.LogError(e, "Update of branch {BranchName} failed", branch.Name);
						branch.Broken = true;
					}
					db.Update(branch);
				}
				else
				{
					Logger.LogInformation("Branch {BranchName} is up to date", branch.Name);
				}
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

		private Key FindOrInsertKey(string path, string keyParts, HistoryRepository db)
		{
			var key = db.FirstOrDefault<Key>(x2 => x2.Path == path && x2.KeyParts == keyParts);
			if (key == null)
			{
				key = new Key
				{
					Id = db.AutoIdGenerator.GenerateNewId<Key>(),
					Path = path,
					KeyParts = keyParts
				};
				db.TransactionManager.AddRollbackRemove<Key>(key.Id);
				db.Insert(key.Id);
			}
			return key;
		}

		private async Task<Commit> EnsureChangesCalculated(GitCommit gitCommit)
		{
			using (var db = HistoryRepositoryFactory())
			{
				var commit = db.FirstOrDefault<Commit>(x => x.Hash == gitCommit.Sha);
				if (commit != null)
				{
					// Already calculated
					return commit;
				}

				var gitParents = gitCommit.Parents.ToImmutableArray();

				foreach (var parent in gitParents)
					await EnsureChangesCalculated(parent, db);

				var parents = gitParents
					.Select(x => db.First<Commit>(x2 => x2.Hash == x.Sha))
					.ToImmutableArray();

				commit = new Commit
				{
					Id = db.AutoIdGenerator.GenerateNewId<Commit>(),
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
						Id = db.AutoIdGenerator.GenerateNewId<Change>(),
						CommitId = commit.Id,
						KeyId = keyId,
						Value = value
					});

				if (parents.IsEmpty)
				{
					foreach (var extracted in await ExtractStrings(gitCommit))
					{
						var key = FindOrInsertKey(extracted.Item1, extracted.Item2, db);
						AddChange(key.Id, extracted.Item3);
					}
				}
				else if (parents.Length == 1)
				{
					var parentValues = await ExtractStrings(gitParents.Single());
					var keys = parentValues.Concat(newValues)
						.Select(x => FindOrInsertKey(x.Item1, x.Item2, db))
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
						parentHistories.Add(await db.GetChangesHistory(parent));

					var commonBaseValues = new List<Tuple<string, string, string>>();

					if (commonBase != null)
					{
						commonBaseValues.AddRange(await ExtractStrings(commonBase));
						var commonBaseCommit = db.First<Commit>(x2 => x2.Hash == commonBase.Sha);

						for (var i = 0; i < parentHistories.Count; i++)
						{
							parentHistories[i] = parentHistories[i]
								.TakeWhile(x => x.CommitId != commonBaseCommit.Id)
								.ToImmutableArray();
						}
					}

					var keys = parentHistories
						.SelectMany(x => x)
						.Select(x => db.SingleById<Key>(x.KeyId))
						.Concat(newValues.Concat(commonBaseValues)
							.Select(x => FindOrInsertKey(x.Item1, x.Item2, db)))
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

				db.TransactionManager.AddRollbackRemove<Change>(changes.Select(x => (BsonValue)x.Id));
				db.Insert(changes);
				db.TransactionManager.AddRollbackRemove<Commit>(commit.Id);
				db.Insert(commit);

				if (!newValues.SetEquals(
					(await db.GetAggregatedChanges(commit)).Select(x =>
					{
						var key = db.SingleById<Key>(x.KeyId);
						return new Tuple<string, string, string>(key.Path, key.KeyParts, x.Value);
					})))
				{
					Logger.LogError("Extracted changes mismatch with commit values");
				}

				return commit;
			}
		}




		private IEnumerable<TResult> JoinChanges<TItem, TKey, TResult>(IEnumerable<TItem> left, IEnumerable<TItem> right,
			Func<TItem, TKey> keySelector, Func<TKey, TItem, TItem, TResult> resultSelector)
		{
			return left.FullGroupJoin(right,
				keySelector,
				keySelector,
				(key, x, y) => resultSelector(key, x.SingleOrDefault(), y.SingleOrDefault()));
		}

		private async Task UpdateBranch(Branch branch, CREDContext dbContext)
		{
			//Repository.Network.Fetch(branch.Name,
			//Repository.Network.Remotes[branch.Name].FetchRefSpecs.Select(x => x.Specification), new FetchOptions
			//{
			//	OnProgress = OnProgress
			//}, "RefUpdate");

			//var branchOnlyCommits = Repository.Branches[branch.Name + "/" + branch.Name].Tip
			//	.SelfAndDirectParents()
			//	.TakeWhile(commit => commit.Sha != branch.GitRemoteRef)
			//	.ToArray();

			//if (branchOnlyCommits.Last().Parents.All(x => x.Sha != branch.GitRemoteRef))
			//{
			//	Logger.LogWarning($"Non Fast Forward update of {branch.Name} branch.");
			//}

			//var lastCommit = Repository.Commits.FirstOrDefault(x => x.Sha == branch.GitRemoteRef);

			//var lastContent = ExtractContentTree(lastCommit);

			//foreach (var nextCommit in branchOnlyCommits)
			//{
			//	var nextContent = ExtractContentTree(nextCommit);

			//	var changes = JoinChanges(lastContent, nextContent, x => x.ParsedPath,
			//		(path, xFile, yFile) =>
			//		{
			//			if (xFile == null)
			//				return yFile.Items.Value.Select(item => (path, item.Key, item.Value));
			//			if (yFile == null)
			//				return xFile.Items.Value.Select(item => (path, item.Key, (string)null));
			//			return JoinChanges(xFile.Items.Value, yFile.Items.Value,
			//				item => item.Key,
			//				(key, xItem, yItem) => (path, key, yItem.Value));
			//		})
			//		.SelectMany(x => x)
			//		.Select(x => new Change()
			//		{
			//			Key = new Key
			//			{
			//				Path = x.Item1,
			//				KeyParts = x.Item2
			//			},
			//			Author = nextCommit.Author.Email,
			//			Timestamp = nextCommit.Author.When.UtcDateTime,
			//			Value = x.Item3
			//		})
			//		.ToArray();

			//	// Reuse existing keys in database
			//	var existingKeys = (await dbContext.Keys
			//			.Where(key => changes.Any(x => Key.PathKeyPartsComparer.Equals(x.Key, key)))
			//			.ToArrayAsync())
			//		.ToImmutableHashSet();

			//	Parallel.ForEach(changes, change =>
			//		change.Key = existingKeys.FirstOrDefault(x => Key.PathKeyPartsComparer.Equals(x, change.Key)) ?? change.Key
			//	);

			//	// Reuse existing changes in database
			//	var existingChanges = (await dbContext.Changes.Include(x => x.Key)
			//			.Where(change => changes.Contains(change, Change.ChangeComparer))
			//			.ToArrayAsync())
			//		.ToImmutableHashSet();

			//	Parallel.For(0, changes.Length, i =>
			//		changes[i] = existingChanges.FirstOrDefault(x => Change.ChangeComparer.Equals(x, changes[i])) ?? changes[i]
			//	);

			//	var historyItems = existingChanges
			//		.OrderByDescending(x => x.Timestamp)
			//		.Select(x => new HistoryItem
			//		{
			//			Change = x,
			//			GitCommitHash = nextCommit.Sha,
			//			Timestamp = nextCommit.Committer.When.UtcDateTime,
			//			Comitter = nextCommit.Committer.Email,
			//		})
			//	.ToImmutableArray();

			//	var prevHistoryItem = branch.Head;
			//	foreach (var historyItem in historyItems)
			//	{
			//		historyItem.Parent = prevHistoryItem;
			//		prevHistoryItem = historyItem;
			//	}

			//	branch.Head = prevHistoryItem;
			//	branch.GitLastCommitRef = prevHistoryItem.GitCommitHash;

			//	dbContext.Update(branch);
			//	dbContext.AddRange(historyItems);
			//}
		}


		public GitBridge(ILoggerFactory loggerFactory, IHostingEnvironment environment,
			IServiceProvider serviceProvider, Func<CREDContext> dbContextFactory)
		{
			ServiceProvider = serviceProvider;
			DbContextFactory = dbContextFactory;
			Logger = loggerFactory.CreateLogger(typeof(GitBridge));

			Dispatcher.InvokeAsync(() =>
			{
				var repoPath = Path.Combine(environment.ContentRootPath, "bin", "Repository");
				if (!Repository.IsValid(repoPath))
				{
					Logger.LogWarning($"Repository at path \"{repoPath}\" is invalid. Removing...");
					Directory.Delete(repoPath, true);
					Logger.LogInformation($"Initializing new repository at \"{repoPath}\"...");
					Repository.Init(repoPath);
				}
				Repository = new Repository(repoPath);
			});
			Dispatcher.InvokeAsync(() => Task.Run(Init));


			//Repository = new Repository("C:\\Users\\Voron\\Source\\Repos\\test\\test2");
			//var offBranchCommits = Repository.Branches.First().Commits
			//	.Aggregate(Enumerable.Empty<Commit>(), (ids, commit) => ids.Concat(commit.Parents.Skip(1))).ToList();
			//var branchOnlycommits = Repository.Commits.QueryBy(new CommitFilter()
			//{
			//	SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time | CommitSortStrategies.Reverse,
			//	IncludeReachableFrom = Repository.Branches.First().Tip,
			//	ExcludeReachableFrom = offBranchCommits
			//});

			//var branchOnlycommits2 = Repository.Branches.First().Tip.DirectParents();

			////var start = Repository.Branches.First().Tip;
			////while (start != null)
			////{
			////	branchOnlycommits2.Add(start);
			////	start = start.Parents.FirstOrDefault();
			////}


			//UpdateBranch(new CRED2.Model.Branch()
			//{
			//	Name = "test",
			//	//GitRemoteUrl = "https://github.com/antonpup/Aurora.git",
			//	GitRemoteUrl = "https://github.com/libgit2/libgit2sharp.git",
			//	GitRemoteRef = "refs/heads/master",
			//	GitBranch = true,
			//}).Wait();

			//var r = Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git");
			//using (var dbContext = ServiceProvider.GetService<CREDContext>())
			//{
			//	var repState = dbContext.StateStore.Find(nameof(GitRepositoryService));
			//	if (repState == null)
			//	{

			//	}
			//}
			////"https://github.com/Arks-Layer/PSO2ENPatchCSV.git", Path.Combine(Env.ContentRootPath, "Repository")


			//GitRepUrl = gitRepUrl;
			//RepositoryDirectory = repositoryDirectory;
			//if (!Directory.Exists(RepositoryDirectory))
			//	Repository.Clone(gitRepUrl, repositoryDirectory, new CloneOptions
			//	{
			//		IsBare = false,
			//		OnProgress = output =>
			//		{
			//			Console.WriteLine(output);
			//			return true;
			//		}
			//	});
			//string logMessage = "";

			//foreach (Remote remote in Repository.Network.Remotes)
			//{

			//	IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
			//	Commands.Fetch(Repository, remote.Name, refSpecs, new FetchOptions() { OnProgress = OnProgress, OnTransferProgress = OnTransferProgress}, logMessage);


			//}

			//Repository.Reset(ResetMode.Hard, Repository.Branches.First().Tip);
			//Console.OutputEncoding = Encoding.UTF8;
			//var lines = Directory.GetFiles(RepositoryDirectory, "*.csv", SearchOption.AllDirectories)
			//	.SelectMany(file => File.ReadAllLines(file, Encoding.UTF8).Select(y=>file+y)).ToList();
			//var length = 0;
			//var varlength = 0;
			//foreach (var strings in lines)
			//{
			//	var beg = strings.Substring(0, strings.Length - 3).LastIndexOf("\"\"\"") + 3;
			//	var value = strings.Substring(beg, strings.Length - 3 - beg);
			//	//Console.WriteLine(value);
			//	length += beg;
			//	varlength += value.Length;
			//	//if (strings.Substring(0,beg).Count(x => x == ',') != 1)
			//	//	throw new Exception(strings);
			//}

			//var d = lines.Distinct().ToList();
			//Console.WriteLine(lines.Count);
			//Console.WriteLine(d.Count());
			//foreach (string s in d.Where(x => lines.Count(y=> y == x) > 1))
			//{
			//	Console.WriteLine(s);
			//}

			////Commands.Fetch(Repository, Repository.Network.Remotes.First().Name,);
			////Repository.Network.Remotes.
			////Commands.Fetch(Repository.);
			//var re = Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git").First();
			////		Repository.Branches.Add()
			////			Repository.CreateBranch(Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git").First()
			////				.CanonicalName);
			//foreach (var branch in Repository.Branches)
			//{
			//	Console.WriteLine(branch);
			//}
			////Repository.Reset();
			////			Commands.Checkout(Repository,Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git").First().);

			//foreach (var commit in Repository.Commits.QueryBy(new CommitFilter()))
			//{
			//	Console.WriteLine(commit.Id);
			//}

			////LibGit2Sharp.Repository.Clone(ToString(),, new CloneOptions()
			////{

			////})
			//Console.WriteLine(string.Join(Environment.NewLine,
			//	Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git")));
		}

		private bool OnTransferProgress(TransferProgress progress)
		{
			//Console.WriteLine(progress);
			return true;
		}

		private bool OnProgress(string serverProgressOutput)
		{
			Console.WriteLine(serverProgressOutput);
			return true;
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