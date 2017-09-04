using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CRED.Data;
using CRED2.Model;
using LibGit2Sharp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace CRED2.GitRepository
{
	internal static class GitRepositoryServiceExtensionMethods
	{
		public static IEnumerable<Commit> SelfAndDirectParents(this Commit start)
		{
			while (start != null)
			{
				yield return start;
				start = start.Parents.FirstOrDefault();
			}
		}

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


	public sealed partial class Service
	{
		public IServiceProvider ServiceProvider { get; }
		public Func<CREDContext> DbContextFactory { get; }

		private RepositoryDispatcher Dispatcher { get; } = new RepositoryDispatcher();

		private sealed class TreeFile
		{
			private sealed class ParsedPathEqualityComparer : IEqualityComparer<TreeFile>
			{
				public bool Equals(TreeFile x, TreeFile y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;
					return x.ParsedPath.Equals(y.ParsedPath);
				}

				public int GetHashCode(TreeFile obj)
				{
					return obj.ParsedPath.GetHashCode();
				}
			}

			public static IEqualityComparer<TreeFile> ParsedPathComparer { get; } = new ParsedPathEqualityComparer();

			private sealed class ParsedPathTreeItemEqualityComparer : IEqualityComparer<TreeFile>
			{
				public bool Equals(TreeFile x, TreeFile y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;
					return x.ParsedPath.Equals(y.ParsedPath) && Equals(x.TreeItem, y.TreeItem);
				}

				public int GetHashCode(TreeFile obj)
				{
					unchecked
					{
						return (obj.ParsedPath.GetHashCode() * 397) ^ (obj.TreeItem != null ? obj.TreeItem.GetHashCode() : 0);
					}
				}
			}

			public static IEqualityComparer<TreeFile> ParsedPathTreeItemComparer { get; } = new ParsedPathTreeItemEqualityComparer();

			public TreeFile(TreeEntry treeItem)
			{
				TreeItem = treeItem;
				ParsedPath = treeItem.Path.Split(Path.PathSeparator).ToArray();
				Items = new Lazy<ImmutableHashSet<KeyValuePair<ComparableStringArray, string>>>(
					() => ((Blob)treeItem.Target)
						.GetContentStream()
						.ReadLines()
						.Select(ParseItemOrNull)
						.Where(x => x != null)
						.Select(x => x.Value)
						.ToImmutableHashSet()
				);
			}

			public ComparableStringArray ParsedPath { get; }
			public TreeEntry TreeItem { get; }
			public Lazy<ImmutableHashSet<KeyValuePair<ComparableStringArray, string>>> Items { get; }
		}

		public static string[] ParseCsvLineOrNull(string line)
		{
			if (line == null) return null;

			const string quote = "\"\"\"";
			const string separator = ",";
			var values = new List<string>();

			int GetNextSep(int startPos)
			{
				var sepPos = line.IndexOf(separator, startPos, StringComparison.Ordinal);
				return sepPos == -1 ? line.Length : sepPos;
			}

			var pos = 0;
			while (pos < line.Length)
			{
				var quoteOpen = line.IndexOf(quote, pos, StringComparison.Ordinal);
				var sepPos = GetNextSep(pos);
				if (quoteOpen >= 0 && quoteOpen < sepPos)
				{
					pos = quoteOpen + quote.Length;
					var quoteClose = line.IndexOf(quote, pos, StringComparison.Ordinal);
					if (quoteClose < 0)
						return null;
					else
					{
						values.Add(line.Substring(pos, quoteClose - pos));
						pos = quoteClose + quote.Length;

						sepPos = sepPos = GetNextSep(pos);
						pos = sepPos + separator.Length;
					}
				}
				else
				{
					values.Add(line.Substring(pos, sepPos - pos).Trim());
					pos = sepPos + separator.Length;
				}
			}
			return values.ToArray();
		}

		public static bool TryParseItem(string line, out KeyValuePair<ComparableStringArray, string> parsed)
		{
			var parsedValues = ParseCsvLineOrNull(line);
			if (parsedValues != null && parsedValues.Length > 1)
			{
				parsed = new KeyValuePair<ComparableStringArray, string>(parsedValues.Take(parsedValues.Length - 1).ToArray(),
					parsedValues.Last());
				return true;
			}
			return false;
		}

		public static KeyValuePair<ComparableStringArray, string>? ParseItemOrNull(string line)
		{
			if (TryParseItem(line, out var result))
			{
				return result;
			}
			return null;
		}

		private static ImmutableHashSet<TreeFile> ExtractContentTree(Commit commit)
		{
			return commit.Tree.Flatten().AsParallel().Where(x => x.TargetType == TreeEntryTargetType.Blob &&
																 x.Mode == Mode.NonExecutableFile &&
																 string.Equals(Path.GetExtension(x.Name), ".csv",
																	 StringComparison.OrdinalIgnoreCase))
				.Select(x => new TreeFile(x)).ToImmutableHashSet();
		}

		private IEnumerable<TResult> JoinChanges<TItem, TKey, TResult>(IEnumerable<TItem> left, IEnumerable<TItem> right,
			Func<TItem, TKey> keySelector, Func<TKey, TItem, TItem, TResult> resultSelector)
		{
			return left.FullGroupJoin(right,
				keySelector,
				keySelector,
					(key, x, y) => resultSelector(key, x.SingleOrDefault(), y.SingleOrDefault()));
		}

		private async Task UpdateBranch(BranchHead branchHead, CREDContext dbContext)
		{
			Repository.Network.Fetch(branchHead.Name,
			Repository.Network.Remotes[branchHead.Name].FetchRefSpecs.Select(x => x.Specification), new FetchOptions
			{
				OnProgress = OnProgress
			}, "RefUpdate");

			var branchOnlyCommits = Repository.Branches[branchHead.Name + "/" + branchHead.Name].Tip
				.SelfAndDirectParents()
				.TakeWhile(commit => commit.Sha != branchHead.GitRemoteRef)
				.ToArray();

			if (branchOnlyCommits.Last().Parents.All(x => x.Sha != branchHead.GitRemoteRef))
			{
				Logger.LogWarning($"Non Fast Forward update of {branchHead.Name} branch.");
			}

			var lastCommit = Repository.Commits.FirstOrDefault(x => x.Sha == branchHead.GitRemoteRef);

			var lastContent = ExtractContentTree(lastCommit);

			foreach (var nextCommit in branchOnlyCommits)
			{
				var nextContent = ExtractContentTree(nextCommit);

				var changes = JoinChanges(lastContent, nextContent, x => x.ParsedPath,
					(path, xFile, yFile) =>
					{
						if (xFile == null)
							return yFile.Items.Value.Select(item => (path, item.Key, item.Value));
						if (yFile == null)
							return xFile.Items.Value.Select(item => (path, item.Key, (string)null));
						return JoinChanges(xFile.Items.Value, yFile.Items.Value,
							item => item.Key,
							(key, xItem, yItem) => (path, key, yItem.Value));
					})
					.SelectMany(x => x)
					.Select(x => new Change()
					{
						Key = new Key
						{
							Path = x.Item1,
							KeyParts = x.Item2
						},
						Author = nextCommit.Author.Email,
						Timestamp = nextCommit.Author.When.UtcDateTime,
						Value = x.Item3
					})
					.ToArray();

				// Reuse existing keys in database
				var existingKeys = (await dbContext.Keys
						.Where(key => changes.Any(x => Key.PathKeyPartsComparer.Equals(x.Key, key)))
						.ToArrayAsync())
					.ToImmutableHashSet();

				Parallel.ForEach(changes, change =>
					change.Key = existingKeys.FirstOrDefault(x => Key.PathKeyPartsComparer.Equals(x, change.Key)) ?? change.Key
				);

				// Reuse existing changes in database
				var existingChanges = (await dbContext.Changes.Include(x => x.Key)
						.Where(change => changes.Contains(change, Change.ChangeComparer))
						.ToArrayAsync())
					.ToImmutableHashSet();

				Parallel.For(0, changes.Length, i =>
					changes[i] = existingChanges.FirstOrDefault(x => Change.ChangeComparer.Equals(x, changes[i])) ?? changes[i]
				);

				var historyItems = existingChanges
					.OrderByDescending(x => x.Timestamp)
					.Select(x => new HistoryItem
					{
						Change = x,
						GitCommitHash = nextCommit.Sha,
						Timestamp = nextCommit.Committer.When.UtcDateTime,
						Comitter = nextCommit.Committer.Email,
					})
				.ToImmutableArray();

				var prevHistoryItem = branchHead.Head;
				foreach (var historyItem in historyItems)
				{
					historyItem.Parent = prevHistoryItem;
					prevHistoryItem = historyItem;
				}

				branchHead.Head = prevHistoryItem;
				branchHead.GitLastCommitRef = prevHistoryItem.GitCommitHash;

				dbContext.Update(branchHead);
				dbContext.AddRange(historyItems);
			}
		}

		private async Task Init()
		{
			using (var dbContext = DbContextFactory())
			{
				await dbContext.Heads.Where(head => head.GitBranch)
					.ForEachAsync(head =>
					{
						string refSpec = $"+{head.GitRemoteRef}:refs/remotes/{head.Name}/{head.Name}";

						Remote remote = Repository.Network.Remotes.FirstOrDefault(x => x.Name == head.Name);

						if (remote != null &&
							(remote.FetchRefSpecs.Count() != 1
							 || remote.FetchRefSpecs.First().Specification != refSpec
							 || remote.Url != head.GitRemoteUrl))
						{
							Repository.Network.Remotes.Remove(head.Name);
							remote = null;
						}

						if (remote == null)
							Repository.Network.Remotes.Add(head.Name, head.GitRemoteUrl, refSpec);
					});
			}
		}

		public GitRepositoryService(ILoggerFactory loggerFactory, IHostingEnvironment environment,
			IServiceProvider serviceProvider, Func<CREDContext> dbContextFactory)
		{
			ServiceProvider = serviceProvider;
			DbContextFactory = dbContextFactory;
			Logger = loggerFactory.CreateLogger(typeof(GitRepositoryService));

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


			Repository = new Repository("C:\\Users\\Voron\\Source\\Repos\\test\\test2");
			var offBranchCommits = Repository.Branches.First().Commits
				.Aggregate(Enumerable.Empty<Commit>(), (ids, commit) => ids.Concat(commit.Parents.Skip(1))).ToList();
			var branchOnlycommits = Repository.Commits.QueryBy(new CommitFilter()
			{
				SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time | CommitSortStrategies.Reverse,
				IncludeReachableFrom = Repository.Branches.First().Tip,
				ExcludeReachableFrom = offBranchCommits
			});

			var branchOnlycommits2 = Repository.Branches.First().Tip.DirectParents();

			//var start = Repository.Branches.First().Tip;
			//while (start != null)
			//{
			//	branchOnlycommits2.Add(start);
			//	start = start.Parents.FirstOrDefault();
			//}


			UpdateBranch(new CRED2.Model.BranchHead()
			{
				Name = "test",
				//GitRemoteUrl = "https://github.com/antonpup/Aurora.git",
				GitRemoteUrl = "https://github.com/libgit2/libgit2sharp.git",
				GitRemoteRef = "refs/heads/master",
				GitBranch = true,
			}).Wait();

			var r = Repository.ListRemoteReferences("https://github.com/antonpup/Aurora.git");
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
			//foreach (var branchHead in Repository.Branches)
			//{
			//	Console.WriteLine(branchHead);
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

		private ILogger Logger { get; }
		private Repository Repository { get; set; }
	}
}