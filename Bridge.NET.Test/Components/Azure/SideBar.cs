using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class SideBar : Component<SideBar.Props, SideBar.State>
	{
		public SideBar(bool expanded)
			: base(new Props(expanded)) { }

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

			return DOM.Div(Fxs.ClassAttribute(Fxs.Sidebar),
				DOM.Button(new ButtonAttributes
				{
					ClassName = Fluent.ClassName().Add(Fxs.SidebarCollapseButton, Fxs.HasHover),
					Title = "",
					OnClick = e => SetState(state.With(_ => _.Expanded, !state.Expanded))
					
				}, new Svg(FxSymbols.Hamburger)),

				DOM.Div(Fxs.ClassAttribute(Fxs.Topbar)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalTip)),
				DOM.Div(Fxs.ClassAttribute(Fxs.PortalMain)),
				//contextpane
				DOM.Div(Fxs.ClassAttribute(Fxs.Sidebar))
				//contextpane
			);
		}

		public class Props : IAmImmutable
		{
			public Props(bool expanded)
			{
				this.CtorSet(_ => _.Expanded, expanded);
			}
			public bool Expanded { get; }
		}

		public class State : IAmImmutable
		{
			public State(bool expanded)
			{
				this.CtorSet(_ => _.Expanded, expanded);
			}
			public bool Expanded { get; }
		}

	}
}