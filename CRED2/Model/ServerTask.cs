namespace CRED2.Model
{
    public enum ServerTaskState
    {
        Created,

        Queued,

        Running,

        ConcurencyHit,

        Done,

        Aborted,

        Error,

        AwaitingData,
    }

    public sealed class ServerTask
    {
        public string Data { get; set; }

        public byte[] Id { get; set; }

        public string Initiator { get; set; }

        public ServerTaskState State { get; set; }
    }
}