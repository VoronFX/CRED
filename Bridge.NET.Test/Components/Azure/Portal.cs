using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bridge.NET.Test.Helpers;
using Bridge.NET.Test.ViewModels;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class Portal : PureComponent<Portal.Props>
	{
		public Portal(PortalTheme theme)
			: base(new Props(theme)) { }

		//private enum FxsContainerClasses { FxsPortal, DesktopNormal, FxsShowStartboard }
		private static class Fxs
		{
			// Check for typos
			static Fxs()
			{
				foreach (var property in typeof(Fxs).GetProperties())
				{
					Console.WriteLine(property.Name);
					if ((string)property.GetValue(typeof(Fxs)) != nameof(Fxs) + property.Name)
					{
						throw new ArgumentException($"Property name of \"{property.GetValue(typeof(Fxs))}\""
												  + $" not equal to \"{nameof(Fxs) + property.Name} \". Possible typo was made.");
					}
				}
			}

			public static string Portal => nameof(Fxs) +"-"+ nameof(Portal);
			public static string DesktopNormal => nameof(Fxs) +"-"+ nameof(DesktopNormal);
			public static string ShowStartboard => nameof(Fxs) +"-"+ nameof(ShowStartboard);
			public static string ShowJourney => nameof(Fxs) +"-"+ nameof(ShowJourney);
			public static string Topbar => nameof(Fxs) +"-"+ nameof(Topbar);
			public static string SideBar => nameof(Fxs) +"-"+ nameof(SideBar);
				    
			public static string PortalTip => nameof(Fxs) +"-"+ nameof(PortalTip);
			public static string PortalMain => nameof(Fxs) +"-"+ nameof(PortalMain);
			public static string PortalContent => nameof(Fxs) +"-"+ nameof(PortalContent);
				    
			public static string Trim => nameof(Fxs) +"-"+ nameof(Trim);
				  
			public static string ScrollbarTransparent => nameof(Fxs) +"-"+ nameof(ScrollbarTransparent);
			public static string ScrollbarDefaultHover => nameof(Fxs) +"-"+ nameof(ScrollbarDefaultHover);
			public static string Panorama => nameof(Fxs) +"-"+ nameof(Panorama);


			public static string SelectClasses(params string[] names)
				=> string.Join(" ", names.Select(x => x.ToLower()));

			public static Attributes ClassAttribute(params string[] names)
				=> new Attributes { ClassName = SelectClasses(names) };
		}

		public override ReactElement Render()
		{
			//fxs - portal fxs - desktop - normal fxs - show - startboard
			//"<div id="web - container" class="fxs - portal fxs - desktop - normal fxs - show - journey">
			//	< div class="fxs-topbar">
			//	</div>
			//	<div class="fxs-portal-tip"></div>
			//	<div class="fxs-portal-main">
			//	<div class="fxs-sidebar fxs-trim"></div>
			//	<div class="portal-contextpane" alignment="left"></div>
			//	<div class="fxs-portal-content fxs-scrollbar-transparent fxs-scrollbar-default-hover fxs-panorama"></div>
			//	<div class="portal-contextpane" alignment="right"></div>
			//	</div>
			//	<azure-svg-symbols></azure-svg-symbols>
			//	</div>
			//	"

			return DOM.Div(new Attributes
			{
				Id = "web-container",
				ClassName = Fxs.SelectClasses(Fxs.Portal, Fxs.DesktopNormal, Fxs.ShowStartboard)
			}, new[]
				{
				DOM.Div(Fxs.ClassAttribute(Fxs.Topbar)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalTip)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalMain)),
				//contextpane
				DOM.Div(Fxs.ClassAttribute(Fxs.SideBar)),
				//contextpane
				});
		}

		public class Props : IAmImmutable
		{
			public Props(PortalTheme theme)
			{
				this.CtorSet(_ => _.Theme, theme);
			}
			public PortalTheme Theme { get; }
		}

		public enum PortalTheme
		{
			Azure, Blue, Light, Black
		}
	}
}
