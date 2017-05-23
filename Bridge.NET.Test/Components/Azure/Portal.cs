using AzurePortal;
using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using CRED.Client.Helpers;
using ProductiveRage.Immutable;

namespace CRED.Client.Components.Azure
{
	public sealed class Portal : PureComponent<Portal.Props>
	{
		public Portal(Fxs fxs, PortalTheme theme, bool showStartboard)
			: base(new Props(fxs, theme, showStartboard))
		{
		}

		public override ReactElement Render()
		{
			return
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsThemeDark, Classes.FxsModeDark, Classes.ExtModeDark)
						.AddIf(_ => props.Theme == PortalTheme.Azure, Classes.FxsThemeAzure)
						.AddIf(_ => props.Theme == PortalTheme.Blue, Classes.FxsThemeBlue)
						.AddIf(_ => props.Theme == PortalTheme.Light, Classes.FxsThemeLight)
						.AddIf(_ => props.Theme == PortalTheme.Black, Classes.FxsThemeDark, Classes.FxsModeDark, Classes.ExtModeDark)
						.AddIf(_ => props.Theme != PortalTheme.Black, Classes.FxsModeLight, Classes.ExtModeLight),
					Style = new ReactStyle { Height = "100%" }
				},
					DOM.Div(new Attributes
					{
						Id = "web-container",
						ClassName = Fluent.ClassName(Classes.FxsPortal, Classes.FxsDesktopNormal)
								.Add(props.ShowStartboard ? Classes.FxsShowStartboard : Classes.FxsShowJourney)
					},
						new Topbar(props.Fxs, true, true),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsPortalTip)
						}),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsPortalMain),
						},
							new Sidebar(props.Fxs, NonNullList<Sidebar.SideBarButton>.Empty,
								NonNullList<Sidebar.SideBarButton>.Empty)
						)
					)
				);
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