using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CRED2.Model;
using LiteDB;
using Microsoft.Extensions.Caching.Memory;
using MoreLinq;

namespace CRED2.GitRepository
{
	public sealed class HistoryRepository : LiteRepository, IDisposable
	{
		private string AggregatedChangesCacheKey(string commitHash) => $"{commitHash}AggregatedChanges";
		private string ValuesCacheKey(string commitHash) => $"{commitHash}Values";
		private string CommitsHistoryCacheKey(string commitHash) => $"{commitHash}CommitsHistory";
		private string ChangesHistoryCacheKey(string commitHash) => $"{commitHash}ChangesHistory";
		private string ChangesCacheKey(string commitHash) => $"{commitHash}Changes";

		private readonly Lazy<AutoIdGenerator> autoIdGenerator;
		private bool disposed;
		private IMemoryCache MemoryCache { get; }

		public HistoryRepository(ConnectionString connectionString, IMemoryCache memoryCache, BsonMapper mapper = null) : base(connectionString, mapper)
		{
			MemoryCache = memoryCache;
			TransactionManager = new TransactionManager(this);
			autoIdGenerator = new Lazy<AutoIdGenerator>(() => new AutoIdGenerator(this));
		}

		public TransactionManager TransactionManager { get; }
		public AutoIdGenerator AutoIdGenerator => autoIdGenerator.Value;

		public Task<Commit> GetCommit(long id)
		{
			return Task.FromResult(SingleById<Commit>(id));
		}

		public Task<ImmutableArray<Commit>> GetCommits(IList<long> ids)
		{
			return Task.FromResult(Fetch<Commit>(x => ids.Contains(x.Id))
				.OrderBy(x => ids.IndexOf(x.Id))
				.ToImmutableArray());
		}

		public Task<ImmutableHashSet<Change>> GetChanges(Commit commit)
		{
			return MemoryCache.GetOrCreateAsync(
				ChangesCacheKey(commit.Hash), entry =>
				{
					entry.Priority = CacheItemPriority.Low;
					return Task.Run(() =>
						Fetch<Change>(x => x.CommitId == commit.Id)
							.ToImmutableHashSet());
				});
		}

		public Task<ImmutableHashSet<Change>> GetAggregatedChanges(Commit commit)
		{
			return MemoryCache.GetOrCreateAsync(
				AggregatedChangesCacheKey(commit.Hash), entry =>
				{
					entry.Priority = CacheItemPriority.Low;

					return GetChangesHistory(commit)
					.ContinueWith(task =>
						{
							return task.Result
							.Reverse()
							.Aggregate(new HashSet<Change>(Change.KeyIdComparer), (hashSet, change) =>
							{
								hashSet.Add(change);
								return hashSet;
							})
							.ToImmutableHashSet();
						});
				});
		}

		public Task<ImmutableArray<Commit>> GetCommitsHistory(Commit commit)
		{
			if (commit.Parents.Length == 0)
				return Task.FromResult(new ImmutableArray<Commit> { commit });
			return MemoryCache.GetOrCreateAsync(
				CommitsHistoryCacheKey(commit.Hash), entry =>
				{
					entry.Priority = CacheItemPriority.Low;
					return Task.Run(async () =>
					{
						var history = new List<Commit> { commit };
						if (commit.Parents.Length == 1)
							history.AddRange(await GetCommitsHistory(await GetCommit(commit.Parents.Single())));
						else
						{
							var histories = new List<Queue<Commit>>();
							foreach (var parent in commit.Parents)
								histories.Add(new Queue<Commit>(await GetCommitsHistory(await GetCommit(parent))));

							while (histories.Any(x => x.Any()))
							{
								Queue<Commit> bestQueue = null;
								DateTimeOffset bestTimestamp = DateTimeOffset.MinValue;

								foreach (var parentHistory in histories)
								{
									if (parentHistory.TryPeek(out var nextCommit)
										&& nextCommit.Committer.When > bestTimestamp)
									{
										if (history.Contains(nextCommit, Commit.IdComparer))
										{
											parentHistory.Clear();
										}
										else
										{
											bestQueue = parentHistory;
											bestTimestamp = nextCommit.Committer.When;
										}
									}
								}
								if (bestQueue != null)
									history.Add(bestQueue.Dequeue());
							}
						}
						return history.ToImmutableArray();
					});
				});
		}

		public Task<ImmutableArray<Change>> GetChangesHistory(Commit commit)
		{
			return MemoryCache.GetOrCreateAsync(
				ChangesHistoryCacheKey(commit.Hash), entry =>
				{
					entry.Priority = CacheItemPriority.Low;
					return Task.Run(async () =>
					{
						var commitHistory = await GetCommitsHistory(commit);
						var history = new List<Change>();
						foreach (var historyCommit in commitHistory)
						{
							history.AddRange(await GetChanges(historyCommit));
						}
						return history.ToImmutableArray();
					});
				});
		}


		public new void Dispose()
		{
			if (disposed)
				return;
			disposed = true;
			TransactionManager.Dispose();
			base.Dispose();
		}
	}
}