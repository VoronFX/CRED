using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzurePortal;
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
			: base(new Props(fxs, theme, showStartboard))
		{
		}

		public override ReactElement Render()
		{
			//return React.DOM.div({
			//	id: "web-container",
			//	className: Bridge.NET.Test.Helpers.Fluent.FluentClassName.op_Implicit(Bridge.NET.Test.Helpers.Fluent.ClassName([this.Classes.FxsPortal, this.Classes.FxsDesktopNormal]).Add([this.unwrappedProps.ShowStartboard ? this.Classes.FxsShowStartboard : this.Classes.FxsShowJourney]))
			//}, React.DOM.div({
			//	title: "sdadasdasd"

			//}), React.DOM.div({ }));

			//Union<ReactElement, string>[] children

			var a = DOM.Div(new Attributes
			{
				Id = "web-container",
				ClassName = Fluent.ClassName(Classes.FxsPortal, Classes.FxsDesktopNormal)
						.Add(props.ShowStartboard ? Classes.FxsShowStartboard : Classes.FxsShowJourney)
			},
			DOM.Div(new Attributes(){Title = "sdadasdasd"}),
			DOM.Div(new Attributes(){Title = "ww"}),
			DOM.Div(new Attributes(){Title = "sdadasdasd"}),
			DOM.Div(new Attributes())
			
			//Fluent.ChildrenBuilder()
			//	.Add(DOM.Div(new Attributes()))
			//	.Add(DOM.Div(new Attributes()
			//	{

			//	}))
			//	//.Add(new Topbar(props.Fxs, true, true))
			//	//.Add(new Sidebar(props.Fxs, NonNullList<Sidebar.SideBarButton>.Empty,
			//	//	NonNullList<Sidebar.SideBarButton>.Empty))
			//	.Build().Cast<Union<ReactElement, string>>().ToArray()

			//DOM.Div(Classes.FxsClassAttribute(Classes.FxsTopbar)),
			//DOM.Div(Classes.FxsClassAttribute(Classes.FxsPortalTip)),
			//DOM.Div(Classes.FxsClassAttribute(Classes.FxsPortalMain)),
			////contextpane
			//DOM.Div(Classes.FxsClassAttribute(Classes.FxsSidebar)),
			//DOMEx.Svg(Classes.FxsClassAttribute(Classes.FxsSidebar)),

			//contextpane
			);
			return a;
		}

		private StyleClassesMap Classes => props.Fxs.StyleClasses;
		private DummyClassesMap DummyClasses => props.Fxs.DummyClasses;
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
			Azure,
			Blue,
			Light,
			Black
		}
	}
}