﻿using System;
using System.Collections.Generic;
using System.Linq;
using CRED2.Model;
using LiteDB;

namespace CRED2.GitRepository
{
	public sealed class TransactionManager : IDisposable
	{
		public LiteRepository Repository { get; }

		private bool disposed;
		private TransactionState TransactionState { get; set; }
		public bool InTransaction => TransactionState != null;

		public TransactionManager(LiteRepository repository)
		{
			Repository = repository;
		}

		public void Commit()
		{

			TransactionState.Complete = true;
			Repository.Update(TransactionState);
			Repository.Delete<TransactionRollbackItem>(x => x.TransactionId == TransactionState.Id);
			Repository.Delete<TransactionState>(TransactionState.Id);
			TransactionState = null;
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

		public void Rollback()
		{
			foreach (var rollbackItem in Repository.Fetch<TransactionRollbackItem>(x => x.TransactionId == TransactionState.Id))
			{
				if (rollbackItem.UpsertDocument != null)
					Repository.Upsert(rollbackItem.UpsertDocument, rollbackItem.CollectionName);
				else
					Repository.Database.GetCollection(rollbackItem.CollectionName).Delete(rollbackItem.RemoveDocumentId);
			}
			Repository.Delete<TransactionRollbackItem>(x => x.TransactionId == TransactionState.Id);
			Repository.Delete<TransactionState>(TransactionState.Id);
			TransactionState = null;
		}

		public void EnsureDataConsistence()
		{
			foreach (var transactionState in Repository.Fetch<TransactionState>(x => !x.Complete))
			{
				TransactionState = transactionState;
				Rollback();
			}
			foreach (var transactionState in Repository.Fetch<TransactionState>(x => x.Complete))
			{
				TransactionState = transactionState;
				Commit();
			}
		}

		public void Enter()
		{
			if (InTransaction)
				throw new InvalidOperationException("Transaction already started.");
			TransactionState = new TransactionState();
			Repository.Insert(TransactionState);
		}

		public void Dispose()
		{
			if (disposed || TransactionState == null || TransactionState.Complete)
				return;
			disposed = true;
			Rollback();
		}
	}
}