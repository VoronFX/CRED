using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CRED2.Model;
using LiteDB;

namespace CRED2.GitRepository
{
	public sealed class AutoIdGenerator
	{
		private LiteRepository Repository { get; }

		private ConcurrentDictionary<string, long> LastAutoIds { get; }
		= new ConcurrentDictionary<string, long>();

		public AutoIdGenerator(LiteRepository repository)
		{
			Repository = repository;
		}

		public long GenerateNewId<T>()
		{
			return GenerateNewId(typeof(T).Name);
		}

		public long GenerateNewId(string collectionName)
		{
			return LastAutoIds.AddOrUpdate(collectionName, key =>
			{
				var fromDb = Repository.Database.GetCollection(collectionName).Max();
				if (!fromDb.IsInt64)
					throw new NotSupportedException("Only Int64 ids are supported in AutoIdGenerator");
				return fromDb + 1;
			}, (key, value) => value + 1);
		}
	}
}