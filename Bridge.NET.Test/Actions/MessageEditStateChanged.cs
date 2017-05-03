using Bridge.NET.Test.ViewModels;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Actions
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
