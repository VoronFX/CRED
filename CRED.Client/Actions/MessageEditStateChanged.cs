using Bridge.React;
using CRED.Client.ViewModels;
using ProductiveRage.Immutable;

namespace CRED.Client.Actions
{
	public class MessageEditStateChanged : IDispatcherAction, IAmImmutable
	{
		public MessageEditStateChanged(MessageEditState newState)
		{
			this.CtorSet(_ => _.NewState, newState);
		}
		public MessageEditState NewState { get; }
	}
}
