using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CRED2.Model;
using LiteDB;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace CRED2.GitRepository
{
    public sealed class HistoryRepository : LiteRepository, IDisposable
    {
        private bool disposed;

        public HistoryRepository(IConfiguration configuration, IMemoryCache memoryCache) :
            base(configuration.GetConnectionString(nameof(HistoryRepository)))
        {
            MemoryCache = memoryCache;
            Transaction.EnsureTransactionsCompleted(this);
            AutoIdGenerator = new AutoIdGenerator(this);
        }

        private IMemoryCache MemoryCache { get; }

        public AutoIdGenerator AutoIdGenerator { get; }

        public new void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            base.Dispose();
        }

        private static string AggregatedChangesCacheKey(string commitHash)
        {
            return $"{commitHash}AggregatedChanges";
        }

        private static string ValuesCacheKey(string commitHash)
        {
            return $"{commitHash}Values";
        }

        private static string CommitsHistoryCacheKey(string commitHash)
        {
            return $"{commitHash}CommitsHistory";
        }

        private static string ChangesHistoryCacheKey(string commitHash)
        {
            return $"{commitHash}ChangesHistory";
        }

        private static string ChangesCacheKey(string commitHash)
        {
            return $"{commitHash}Changes";
        }

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
                return Task.FromResult(new ImmutableArray<Commit> {commit});
            return MemoryCache.GetOrCreateAsync(
                CommitsHistoryCacheKey(commit.Hash), entry =>
                {
                    entry.Priority = CacheItemPriority.Low;
                    return Task.Run(async () =>
                    {
                        var history = new List<Commit> {commit};
                        if (commit.Parents.Length == 1)
                        {
                            history.AddRange(await GetCommitsHistory(await GetCommit(commit.Parents.Single())));
                        }
                        else
                        {
                            ImmutableArray<Queue<Commit>> histories = (await Task.WhenAll(commit.Parents.Select(x =>
                                    Task.Run(async () =>
                                        new Queue<Commit>(await GetCommitsHistory(await GetCommit(x)))
                                    ))))
                                .ToImmutableArray();

                            while (histories.Any(x => x.Any()))
                            {
                                Queue<Commit> bestQueue = null;
                                DateTimeOffset bestTimestamp = DateTimeOffset.MinValue;

                                foreach (Queue<Commit> parentHistory in histories)
                                    if (parentHistory.TryPeek(out Commit nextCommit)
                                        && nextCommit.Committer.When > bestTimestamp)
                                        if (history.Contains(nextCommit, Commit.IdComparer))
                                        {
                                            parentHistory.Clear();
                                        }
                                        else
                                        {
                                            bestQueue = parentHistory;
                                            bestTimestamp = nextCommit.Committer.When;
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
                        ImmutableArray<Commit> commitHistory = await GetCommitsHistory(commit);
                        var history = new List<Change>();

                        return (await Task.WhenAll(commitHistory.Select(x =>
                            Task.Run(async () => await GetChanges(x)
                            ))))
                            .SelectMany(x => x)
                            .ToImmutableArray();
                    });
                });
        }
    }
}