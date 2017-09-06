using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRED2.Model
{
    public sealed class ServerTask
    {
		public byte[] Id { get; set; }

		public string Initiator { get; set; }

		public ServerTaskState State { get; set; }

		public string Data { get; set; }

    }

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
}
