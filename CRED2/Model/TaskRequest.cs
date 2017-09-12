using System;

using LiteDB;

namespace CRED2.Model
{
    public enum TaskRequestState
    {
        Created,

        RequestRecieved,

        Complete
    }

    public sealed class TaskRequest
    {
        public DateTime Created { get; set; }

        public long Id { get; set; }

        public BsonDocument RequestData { get; set; }

        public string RequestDataType { get; set; }

        public BsonDocument Response { get; set; }

        public TaskRequestState State { get; set; }
    }
}