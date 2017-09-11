using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace CRED2.Model
{
    public sealed class TaskRequest
    {
        public long Id { get; set; }

        public DateTime Created { get; set; }

        public TaskRequestState State { get; set; }

        public string RequestDataType { get; set; }

        public BsonDocument RequestData { get; set; }

        public BsonDocument Response { get; set; }

    }

    public enum TaskRequestState
    {
        Created,
        RequestRecieved,
        Complete
    }
}
