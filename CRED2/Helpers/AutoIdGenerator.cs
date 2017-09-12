using System;
using System.Collections.Concurrent;

using LiteDB;

namespace CRED2.Helpers
{
    public sealed class AutoIdGenerator
    {
        public AutoIdGenerator(LiteRepository repository)
        {
            this.Repository = repository;
        }

        private ConcurrentDictionary<string, long> LastAutoIds { get; } = new ConcurrentDictionary<string, long>();

        private LiteRepository Repository { get; }

        public long GenerateNewId<T>()
        {
            return this.GenerateNewId(typeof(T).Name);
        }

        public long GenerateNewId(string collectionName)
        {
            return this.LastAutoIds.AddOrUpdate(
                collectionName,
                key =>
                    {
                        var fromDb = this.Repository.Database.GetCollection(collectionName).Max();
                        if (!fromDb.IsInt64)
                            throw new NotSupportedException("Only Int64 ids are supported in AutoIdGenerator");
                        return fromDb + 1;
                    },
                (key, value) => value + 1);
        }
    }
}