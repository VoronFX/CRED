using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using CRED2.Model;

using LiteDB;

namespace CRED2.Helpers
{
    public sealed class Transaction : IDisposable
    {
        private bool disposed;

        public Transaction(LiteRepository repository)
            : this(repository, new TransactionState())
        {
        }

        public Transaction(LiteRepository repository, TransactionState transactionState)
        {
            this.Repository = repository;
            this.TransactionState = transactionState;
            this.Repository.Insert(transactionState);
        }

        public event EventHandler<ImmutableArray<TransactionRollbackItem>> BeforeRollback;

        private LiteRepository Repository { get; }

        private TransactionState TransactionState { get; }

        public static void EnsureTransactionsCompleted(LiteRepository repository)
        {
            foreach (var state in repository.Fetch<TransactionState>())
            {
                new Transaction(repository, state).Dispose();
            }
        }

        public void AddRollbackRemove<T>(BsonValue documentId)
        {
            this.AddRollbackRemove<T>(new[] { documentId });
        }

        public void AddRollbackRemove(BsonValue documentId, string collectionName)
        {
            this.AddRollbackRemove(new[] { documentId }, collectionName);
        }

        public void AddRollbackRemove<T>(IEnumerable<BsonValue> documentIds)
        {
            this.AddRollbackRemove(documentIds, typeof(T).Name);
        }

        public void AddRollbackRemove(IEnumerable<BsonValue> documentIds, string collectionName)
        {
            this.Repository.Insert(
                documentIds.Select(
                    x => new TransactionRollbackItem
                             {
                                 CollectionName = collectionName,
                                 RemoveDocumentId = x,
                                 TransactionId = this.TransactionState.Id
                             }));
        }

        public void AddRollbackRestore<T>(T document, string collectionName)
        {
            this.AddRollbackRestore<T>(new[] { document }, collectionName);
        }

        public void AddRollbackRestore<T>(T document)
        {
            this.AddRollbackRestore<T>(new[] { document });
        }

        public void AddRollbackRestore<T>(IEnumerable<T> documents)
        {
            this.AddRollbackRestore(documents, typeof(T).Name);
        }

        public void AddRollbackRestore<T>(IEnumerable<T> documents, string collectionName)
        {
            this.Repository.Insert(
                documents.Select(
                    x => new TransactionRollbackItem
                             {
                                 CollectionName = collectionName,
                                 UpsertDocument =
                                     this.Repository.Database.Mapper.ToDocument(typeof(T), x),
                                 TransactionId = this.TransactionState.Id
                             }));
        }

        public void Commit()
        {
            if (this.TransactionState.Complete) throw new Exception("Transaction already completed");
            this.TransactionState.Complete = true;
            this.Repository.Update(this.TransactionState);
        }

        public void Dispose()
        {
            if (this.disposed) return;
            this.disposed = true;
            if (!this.TransactionState.Complete) this.Rollback();
            this.Repository.Delete<TransactionRollbackItem>(x => x.TransactionId == this.TransactionState.Id);
            this.Repository.Delete<TransactionState>(this.TransactionState.Id);
        }

        public void Rollback()
        {
            if (this.TransactionState.Complete) throw new Exception("Transaction already completed");
            var rollbackItems = this.Repository
                .Fetch<TransactionRollbackItem>(x => x.TransactionId == this.TransactionState.Id).ToImmutableArray();
            this.BeforeRollback?.Invoke(this, rollbackItems);
            foreach (var rollbackItem in rollbackItems)
            {
                if (rollbackItem.UpsertDocument != null)
                    this.Repository.Upsert(rollbackItem.UpsertDocument, rollbackItem.CollectionName);
                else
                    this.Repository.Database.GetCollection(rollbackItem.CollectionName)
                        .Delete(rollbackItem.RemoveDocumentId);
            }
        }
    }
}