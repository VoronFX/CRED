using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Actions
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
