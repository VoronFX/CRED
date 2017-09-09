using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CRED2.Model;
using LiteDB;

namespace CRED2.GitRepository
{
	public sealed class TransactionManager : IDisposable
	{
		private LiteRepository Repository { get; }
		private ConcurrentDictionary<long, Transaction> Transactions { get; }
		= new ConcurrentDictionary<long, Transaction>();

		private bool disposed;

		public TransactionManager(LiteRepository repository)
		{
			Repository = repository;

			foreach (var state in repository.Fetch<TransactionState>())
			{
				new Transaction(Repository, state).Dispose();
			}
		}

		public Transaction New()
		{
			if (disposed)
				throw new InvalidOperationException("Already disposed");
			var state = new TransactionState();
			Repository.Insert(state);
			var transaction = new Transaction(Repository, state);
			transaction.BeforeRollback += (sender, items) => Transactions.TryRemove(state.Id, out var dummy);
			Transactions.TryAdd(state.Id, transaction);
			return transaction;
		}

		public void Dispose()
		{
			if (disposed)
				return;
			disposed = true;
			foreach (var transaction in Transactions.ToArray())
				transaction.Value.Dispose();
		}

		public sealed class Transaction : IDisposable
		{
			private LiteRepository Repository { get; }
			private bool disposed;
			private TransactionState TransactionState { get; }

			public Transaction(LiteRepository repository, TransactionState transactionState)
			{
				Repository = repository;
				TransactionState = transactionState;
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
}