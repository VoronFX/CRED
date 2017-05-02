using System;
using System.Linq;
using Bridge;
using Bridge.Html5;
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
			React.React.Render(
				DOM.Div(new Attributes { ClassName = "welcome" }, "Hi!"),
				Document.GetElementById("main")
			);
			Window.Eval<object>("window.loadComplete();");

		}
	}
}