using LiteDB;

namespace CRED2.Model
{
	public class TransactionRollbackItem
	{
		public long Id { get; set; }

		public long TransactionId { get; set; }

		public string CollectionName { get; set; }

		public BsonValue RemoveDocumentId { get; set; }

		public BsonDocument UpsertDocument { get; set; }

	}
}