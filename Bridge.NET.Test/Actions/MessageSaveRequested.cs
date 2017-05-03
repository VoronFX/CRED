using Bridge.NET.Test.API;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Actions
{
	public class MessageSaveRequested : IDispatcherAction, IAmImmutable
	{
		public MessageSaveRequested(MessageDetails message)
		{
			this.CtorSet(_ => _.Message, message);
		}
		public MessageDetails Message { get; private set; }
	}
}
