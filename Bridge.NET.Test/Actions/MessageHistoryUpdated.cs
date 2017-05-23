using Bridge.React;
using CRED.Client.API;
using ProductiveRage.Immutable;
using RequestId = ProductiveRage.Immutable.RequestId;

namespace CRED.Client.Actions
{
	public class MessageHistoryUpdated : IDispatcherAction, IAmImmutable
	{

		public MessageHistoryUpdated(RequestId requestId, NonNullList<SavedMessageDetails> messages)
		{
			this.CtorSet(_ => _.RequestId, requestId);
			this.CtorSet(_ => _.Messages, messages);
		}
		public RequestId RequestId { get; }
		public NonNullList<SavedMessageDetails> Messages { get; }
	}
}
