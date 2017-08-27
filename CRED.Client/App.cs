using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bridge;
using Bridge.Html5;
using Bridge.React;
using CRED.Client;
using CRED.Client.Actions;
using CRED.Client.API;
using CRED.Client.Components;
using CRED.Client.Helpers;
using CRED.Client.Stores;
namespace CRED.Client
{
	public class App
	{
		public static void Main()
		{
			Load().ContinueWith(task =>
			{
				if (task.IsFaulted)
					foreach (var exception in task.Exception.InnerExceptions)
					{
						Console.WriteLine(exception);
					}
			});
		}

		public static async Task Load()
		{


//			Document.Body.AppendChild(new HTMLDivElement()
//			{
//				InnerHTML = @"
//   <section class=""section"">
//    <div class=""container"">
//      <h1 class=""title"">
//        Hello World
//      </h1>
//      <p class=""subtitle"">
//        My first website with <strong>Bulma</strong>!
//      </p> <div class=""card"">
//  <div class=""card-image"">
//    <figure class=""image is-4by3"">
//      <img src=""http://bulma.io/images/placeholders/1280x960.png"" alt=""Image"">
//    </figure>
//  </div>
//  <div class=""card-content"">
//    <div class=""media"">
//      <div class=""media-left"">
//        <figure class=""image is-48x48"">
//          <img src=""http://bulma.io/images/placeholders/96x96.png"" alt=""Image"">
//        </figure>
//      </div>
//      <div class=""media-content"">
//        <p class=""title is-4"">John Smith</p>
//        <p class=""subtitle is-6"">@johnsmith</p>
//      </div>
//    </div>

//    <div class=""content"">
//      Lorem ipsum dolor sit amet, consectetur adipiscing elit.
//      Phasellus nec iaculis mauris. <a>@bulmaio</a>.
//      <a>#css</a> <a>#responsive</a>
//      <br>
//      <small>11:09 PM - 1 Jan 2016</small>
//    </div>
//  </div>
//</div>
//    </div>
//  </section>
//"
//			});

			var dispatcher = new AppDispatcher();
			var store = new AppUIStore(dispatcher, new MessageApi(dispatcher));
			var container = new HTMLDivElement();
			Document.Body.InsertBefore(container, Document.Body.FirstChild);

			React.Render(
				new AppContainer(store, dispatcher),
				container
			);

			// After the Dispatcher and the Store and the Container Component are all associated with each other, the Store needs to be told that
			// it's time to set its initial state, so that the Component can receive an OnChange event and draw itself accordingly. In a more
			// complicated app, this would probably be an event fired by the router - initialising the Store appropriate to the current URL,
			// but in this case there's only a single Store to initialise.
			dispatcher.HandleViewAction(new StoreInitialised(store));

			// Turning of spashscreen
			//Window.Eval<object>("window.loadComplete();");
			var splashscreen = Document.GetElementsByClassName("splashscreen-container")
				.First();
			splashscreen.ClassList.Add("splashscreen-container-out");
			await Task.Delay(500).ContinueWith(t2 => splashscreen.Remove());
		}
	}
}