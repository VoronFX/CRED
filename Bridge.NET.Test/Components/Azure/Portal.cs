using System;
using System.Collections;
using System.Collections.Generic;
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
			},
				DOM.Div(Fxs.ClassAttribute(Fxs.Topbar)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalTip)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalMain)),
				//contextpane
				DOM.Div(Fxs.ClassAttribute(Fxs.Sidebar)),
				//DOMEx.Svg(Fxs.ClassAttribute(Fxs.Sidebar)),
				new Svg(FxSymbols.Hamburger)
				//contextpane
				);
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
