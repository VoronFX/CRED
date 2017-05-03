using System;
using System.Linq;
using Bridge.Html5;
using Bridge.NET.Test.Actions;
using Bridge.NET.Test.API;
using Bridge.NET.Test.Components;
using Bridge.NET.Test.Stores;
using Bridge.React;

namespace Bridge.NET.Test
{
	public class App
	{
		public static void Main()
		{
			// Create a new Button
			//var button = new HTMLButtonElement
			//{
			//	InnerHTML = "Click Me",
			//	OnClick = (ev) =>
			//	{
			//		// When Button is clicked, 
			//		// the Bridge Console should open.
			//		Console.WriteLine("Success!");
			//	}
			//};

			//// Add the Button to the page
			//Document.Body.AppendChild(button);

			// To confirm Bridge.NET is working: 
			// 1. Build this project (Ctrl + Shift + B)
			// 2. Browse to file /Bridge/www/demo.html
			// 3. Right-click on file and select "View in Browser" (Ctrl + Shift + W)
			// 4. File should open in a browser, click the "Submit" button
			// 5. Success!
			//var container = Document.GetElementById("main");
			//container.ClassName = string.Join(
			//	" ",
			//	container.ClassName.Split().Where(c => c != "loading")
			//);
			//React.Render(
			//	DOM.Div(new Attributes { ClassName = "welcome" }, "Hi!"),
			//	container
			//);
			//React.React.Render(
			//	DOM.Div(new Attributes { ClassName = "welcome" }, "Hi!"),
			//	Document.GetElementById("main")
			//);
			
			var dispatcher = new AppDispatcher();
			var store = new AppUIStore(dispatcher, new MessageApi(dispatcher));

			var container = Document.GetElementById("main");
			React.React.Render(
				new AppContainer(store, dispatcher), 
				container
			);

			// After the Dispatcher and the Store and the Container Component are all associated with each other, the Store needs to be told that
			// it's time to set its initial state, so that the Component can receive an OnChange event and draw itself accordingly. In a more
			// complicated app, this would probably be an event fired by the router - initialising the Store appropriate to the current URL,
			// but in this case there's only a single Store to initialise.
			dispatcher.HandleViewAction(new StoreInitialised(store));

			// Turning of spashscreen
			Window.Eval<object>("window.loadComplete();");
		}
	}
}