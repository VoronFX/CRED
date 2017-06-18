using Bridge.React;
using CRED.Client.API;
using CRED.Client.Components.Azure;
using CRED.Client.Components.Azure.Resources;
using CRED.Client.Stores;
using CRED.Client.ViewModels;
using ProductiveRage.Immutable;

namespace CRED.Client.Components
{
	public class AppContainer : Component<AppContainer.Props, Optional<AppContainer.State>>
	{
		public AppContainer(AppUIStore store, AppDispatcher dispatcher) : base(new Props(store, dispatcher)) { }

		protected override void ComponentDidMount()
		{
			props.Store.Change += StoreChanged;
		}
		protected override void ComponentWillUnmount()
		{
			props.Store.Change -= StoreChanged;
		}
		private void StoreChanged()
		{
			SetState(new State(
				newMessage: props.Store.NewMessage,
				messageHistory: props.Store.MessageHistory
			));
		}

		public override ReactElement Render()
		{
			// If state has no valueyet, then the Store has not been initialised and its OnChange event has not been called - in this case, we are not ready to
			// render anything and so should return null here
			if (!state.IsDefined)
				return null;

			// A good guideline to follow with stateful components is that the State reference should contain everything required to draw the components and
			// props should only be used to access a Dispatcher reference to deal with callbacks from those components
			return DOM.Div(new Attributes
			{
				Style = new ReactStyle { Height = "100%" }

			}, new Portal(new Fxs(), Portal.PortalTheme.Black, false));
			//return DOM.Div(null,
			//	new MessageEditor(
			//		className: new NonBlankTrimmedString("message"),
			//		message: state.Value.NewMessage,
			//		onChange: newState => props.Dispatcher.HandleViewAction(new MessageEditStateChanged(newState)),
			//		onSave: () =>
			//		{
			//			// No validation is required here since the MessageEditor shouldn't let OnSave be called if the current message state is invalid
			//			// (ie. if either field has a ValidationMessage). In some applications, it is preferred that validation messages not be shown
			//			// until a save request is attempted (in which case some additional validation WOULD be performed here), but this app keeps
			//			// things simpler by showing validation messages for all inputs until they have acceptable values (meaning that the first
			//			// time the form is draw, it has validation messages displayed even though the user hasn't interacted with it yet).
			//			props.Dispatcher.HandleViewAction(new MessageSaveRequested(
			//				new MessageDetails(
			//					new NonBlankTrimmedString(state.Value.NewMessage.Title.Text),
			//					new NonBlankTrimmedString(state.Value.NewMessage.Content.Text)
			//				)
			//			));
			//		}
			//	),
			//	new MessageHistory(className: new NonBlankTrimmedString("history"), messages: state.Value.MessageHistory)
			//);
		}

		public sealed class Props : IAmImmutable
		{
			public Props(AppUIStore store, AppDispatcher dispatcher)
			{
				this.CtorSet(_ => _.Store, store);
				this.CtorSet(_ => _.Dispatcher, dispatcher);
			}
			public AppUIStore Store { get; }
			public AppDispatcher Dispatcher { get; }
		}

		public class State : IAmImmutable
		{
			public State(MessageEditState newMessage, NonNullList<SavedMessageDetails> messageHistory)
			{
				this.CtorSet(_ => _.NewMessage, newMessage);
				this.CtorSet(_ => _.MessageHistory, messageHistory);
			}
			public MessageEditState NewMessage { get; }
			public NonNullList<SavedMessageDetails> MessageHistory { get; }
		}
	}
}
