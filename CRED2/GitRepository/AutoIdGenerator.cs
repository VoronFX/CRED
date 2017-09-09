using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CRED2.Model;
using LiteDB;

namespace CRED2.GitRepository
{
	public sealed class AutoIdGenerator
	{
		public LiteRepository Repository { get; }

		private Dictionary<string, long> LastAutoIds { get; }

		public AutoIdGenerator(LiteRepository repository)
		{
			Repository = repository;
			LastAutoIds = Repository.Fetch<AutoIdStat>()
				.ToDictionary(x => x.CollectionName, x => x.LastAutoId);
		}

		public long GenerateNewId<T>()
		{
			return GenerateNewId(typeof(T).Name);
		}

		public long GenerateNewId(string collectionName)
		{
			if (!LastAutoIds.TryGetValue(collectionName, out var lastId))
				lastId = 1;
			lastId++;
			LastAutoIds[collectionName] = lastId;
			return lastId;
		}

		public void AddToTransaction(TransactionManager transactionManager)
		{
			var existingStats = Repository.Fetch<AutoIdStat>().ToImmutableArray();
			transactionManager.AddRollbackRestore(existingStats);
			transactionManager.AddRollbackRemove<AutoIdStat>(
				LastAutoIds.Where(x => !existingStats.Any(x2 => x2.CollectionName == x.Key)).Select(x => (BsonValue)x.Key));
		}

		public void SaveStats()
		{
			Repository.Upsert(LastAutoIds.Select(x => new AutoIdStat
			{
				CollectionName = x.Key,
				LastAutoId = x.Value
			}));
		}
	}
}