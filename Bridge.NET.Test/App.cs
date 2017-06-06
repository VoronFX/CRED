using System.Reflection;
using Bridge;
using Bridge.Html5;
using Bridge.React;
using CRED.Client.Actions;
using CRED.Client.API;
using CRED.Client.AzureResources;
using CRED.Client.Components;
using CRED.Client.Helpers;
using CRED.Client.Stores;
using ResourceMapper.Base;

namespace CRED.Client
{
	[RequireResource("/lib/react/react.js")]
	[RequireResource("/lib/react/react-dom.js")]
	public class App
	{
		public static void Main()
		{
			var resNode = Document.CreateElement<HTMLDivElement>(TagNames.Div.ToString());
			resNode.Style.Display = Display.None;
			resNode.Id = nameof(resNode);
		
			foreach (string key in Keys(Window.Get(RequireResourceAttribute.ResourcesVariableName)))
			{
				var res = (string)Window.Get(RequireResourceAttribute.ResourcesVariableName)[key];
				if (key.EndsWith(".js"))
					Window.Eval<object>(res);
				else if (key.EndsWith(".css"))
				{
					var style = Document.CreateElement<HTMLStyleElement>(TagNames.Style.ToString());
					style.Type = "text/css";
					style.AppendChild(Document.CreateTextNode(res));
					resNode.AppendChild(style);
				}
				else if (key.EndsWith(".svg"))
				{
					var svg = Document.CreateElement<HTMLDivElement>(TagNames.Svg.ToString());
					svg.InnerHTML = res;
					resNode.AppendChild(svg);
				}
			}
			Document.Head.AppendChild(resNode);
			Window.Set(RequireResourceAttribute.ResourcesVariableName, null);

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

			var container = Document.GetElementById("app-container");
			//var shadow = Script.Eval<HTMLElement>($"{nameof(container)}.attachShadow({{mode: 'open'}});");
			////var shadow = Script.Get<HTMLElement>(container, "attachShadow()");
			////resNode.AppendChild(Document.CreateElement(TagNames.Slot.ToString()));
			//shadow.AppendChild(resNode);
			//container = Document.CreateElement(TagNames.Div.ToString());
			//shadow.AppendChild(container);


			Bridge.React.React.Render(
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