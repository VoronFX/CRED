using LiteDB;

namespace CRED2.Model
{
    public class TransactionRollbackItem
    {
        public string CollectionName { get; set; }

        public long Id { get; set; }

        public BsonValue RemoveDocumentId { get; set; }

        public long TransactionId { get; set; }

        public BsonDocument UpsertDocument { get; set; }
    }
}