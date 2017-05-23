using Bridge.React;
using ProductiveRage.Immutable;

namespace CRED.Client.Actions
{
	public class MessageSaveSucceeded : IDispatcherAction, IAmImmutable
	{
		public MessageSaveSucceeded(RequestId requestId)
		{
			this.CtorSet(_ => _.RequestId, requestId);
		}
		public RequestId RequestId { get; }
	}
}
