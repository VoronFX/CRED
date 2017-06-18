using Bridge.React;
using CRED.Client.API;
using ProductiveRage.Immutable;

namespace CRED.Client.Actions
{
	public class MessageSaveRequested : IDispatcherAction, IAmImmutable
	{
		public MessageSaveRequested(MessageDetails message)
		{
			this.CtorSet(_ => _.Message, message);
		}
		public MessageDetails Message { get; }
	}
}
