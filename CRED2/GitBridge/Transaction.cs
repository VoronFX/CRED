using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CRED2.Model;
using LiteDB;

namespace CRED2.GitRepository
{
	public sealed class Transaction : IDisposable
	{
		private LiteRepository Repository { get; }
		private bool disposed;
		private TransactionState TransactionState { get; }

		public Transaction(LiteRepository repository) : this(repository, new TransactionState())
		{
		}

		public Transaction(LiteRepository repository, TransactionState transactionState)
		{
			Repository = repository;
			TransactionState = transactionState;
			Repository.Insert(transactionState);
		}

		public void AddRollbackRestore<T>(T document, string collectionName)
		{
			AddRollbackRestore<T>(new[] { document }, collectionName);
		}

		public void AddRollbackRestore<T>(T document)
		{
			AddRollbackRestore<T>(new[] { document });
		}

		public void AddRollbackRestore<T>(IEnumerable<T> documents)
		{
			AddRollbackRestore(documents, typeof(T).Name);
		}

		public void AddRollbackRestore<T>(IEnumerable<T> documents, string collectionName)
		{
			Repository.Insert(documents.Select(x => new TransactionRollbackItem
			{
				CollectionName = collectionName,
				UpsertDocument = Repository.Database.Mapper.ToDocument(typeof(T), x),
				TransactionId = TransactionState.Id
			}));
		}

		public void AddRollbackRemove<T>(BsonValue documentId)
		{
			AddRollbackRemove<T>(new[] { documentId });
		}

		public void AddRollbackRemove(BsonValue documentId, string collectionName)
		{
			AddRollbackRemove(new[] { documentId }, collectionName);
		}

		public void AddRollbackRemove<T>(IEnumerable<BsonValue> documentIds)
		{
			AddRollbackRemove(documentIds, typeof(T).Name);
		}

		public void AddRollbackRemove(IEnumerable<BsonValue> documentIds, string collectionName)
		{
			Repository.Insert(documentIds.Select(x => new TransactionRollbackItem
			{
				CollectionName = collectionName,
				RemoveDocumentId = x,
				TransactionId = TransactionState.Id
			}));
		}

		public event EventHandler<ImmutableArray<TransactionRollbackItem>> BeforeRollback;

		public static void EnsureTransactionsCompleted(LiteRepository repository)
		{
			foreach (var state in repository.Fetch<TransactionState>())
			{
				new Transaction(repository, state).Dispose();
			}
		}

		public void Commit()
		{
			if (TransactionState.Complete)
				throw new Exception("Transaction already completed");
			TransactionState.Complete = true;
			Repository.Update(TransactionState);
		}

		public void Rollback()
		{
			if (TransactionState.Complete)
				throw new Exception("Transaction already completed");
			var rollbackItems = Repository
				.Fetch<TransactionRollbackItem>(x => x.TransactionId == TransactionState.Id)
				.ToImmutableArray();
			BeforeRollback?.Invoke(this, rollbackItems);
			foreach (var rollbackItem in rollbackItems)
			{
				if (rollbackItem.UpsertDocument != null)
					Repository.Upsert(rollbackItem.UpsertDocument, rollbackItem.CollectionName);
				else
					Repository.Database.GetCollection(rollbackItem.CollectionName).Delete(rollbackItem.RemoveDocumentId);
			}
		}

		public void Dispose()
		{
			if (disposed)
				return;
			disposed = true;
			if (!TransactionState.Complete)
				Rollback();
			Repository.Delete<TransactionRollbackItem>(x => x.TransactionId == TransactionState.Id);
			Repository.Delete<TransactionState>(TransactionState.Id);
		}
	}
}