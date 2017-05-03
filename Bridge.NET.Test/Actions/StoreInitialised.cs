using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Actions
{
	/// <summary>
	/// This action is raised when the app is ready, when the Dispatcher has been created and the initial Store is ready to be fired up (in a more
	/// complex app
	/// </summary>
	public class StoreInitialised : IDispatcherAction, IAmImmutable	
	{
		public StoreInitialised(object store)
		{
			this.CtorSet(_ => _.Store, store);
		}
		public object Store { get; private set; }
	}
}
