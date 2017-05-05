using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.NET.Test.ViewModels;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public sealed class Portal : PureComponent<Portal.Props>
	{
		public Portal(Fxs fxs, PortalTheme theme, bool showStartboard)
			: base(new Props(fxs, theme, showStartboard)) { }

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
			{
				Id = "web-container",
				ClassName = Fluent.ClassName(Classes.Portal, Classes.DesktopNormal)
					.Add(props.ShowStartboard ? Classes.ShowStartboard : Classes.ShowJourney)
			},
				//DOM.Div(Classes.ClassAttribute(Classes.Topbar)),
				//DOM.Div(Classes.ClassAttribute(Classes.PortalTip)),
				//DOM.Div(Classes.ClassAttribute(Classes.PortalMain)),
				////contextpane
				//DOM.Div(Classes.ClassAttribute(Classes.Sidebar)),
				//DOMEx.Svg(Classes.ClassAttribute(Classes.Sidebar)),
				new Topbar(props.Fxs, true, true)
				//new Sidebar(props.Fxs,
				//NonNullList<Sidebar.SideBarButton>.Empty,
				//NonNullList<Sidebar.SideBarButton>.Empty)
			//new Svg(FxSymbols.Hamburger)
			//contextpane
			);
		}
		private FxsClasses Classes => props.Fxs.Classes;
		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs, PortalTheme theme, bool showStartboard)
			{
				this.CtorSet(_ => _.Fxs, fxs);
				this.CtorSet(_ => _.ShowStartboard, showStartboard);
				this.CtorSet(_ => _.Theme, theme);
			}
			public Fxs Fxs { get; }
			public bool ShowStartboard { get; }
			public PortalTheme Theme { get; }
		}

		public enum PortalTheme
		{
			Azure, Blue, Light, Black
		}
	}
}
