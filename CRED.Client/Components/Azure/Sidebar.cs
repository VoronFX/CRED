using System;
using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using CRED.Client.Helpers;
using ProductiveRage.Immutable;
using Classes = CRED.Client.AzureCssClassesMap;
using DummyClasses = CRED.Client.AzureCssMissingClassesMap;


namespace CRED.Client.Components.Azure
{
	public sealed class Sidebar : Component<Sidebar.Props, Sidebar.State>
	{
		public Sidebar(Fxs fxs, NonNullList<SideBarButton> favorites, NonNullList<SideBarButton> buttons)
			: base(new Props(fxs, favorites, buttons))
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsSidebar, Classes.FxsTrimBorder)
						.AddIf(_ => state.Collapsed, Classes.FxsSidebarIsCollapsed)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarBar, Classes.FxsTrim, Classes.FxsTrimText)
				},
					RenderSidebarTop(),
					RenderSidebarMiddle(),
					RenderSidebarBottom()
				)
			);
		}

		private ReactElement RenderSidebarTop()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsSidebarTop)
			},
				DOM.Button(new ButtonAttributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarCollapseButton, Classes.FxsTrimHover, Classes.FxsTrimSvg),
					Title = Text.ShowMenu,
					OnClick = e => SetState(state.With(_ => _.Collapsed, !state.Collapsed))
				},
					new Svg(Fxs.Symbols.Hamburger)
				),
				DOM.Button(new ButtonAttributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarCreate, Classes.FxsTrimHover, Classes.FxsTrimBorder),
					Title = Text.CreateText,
					//OnClick = e => SetState(state.With(_ => _.Collapsed, !state.Collapsed))
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarButtonFlex)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsSidebarCreateIcon, Classes.FxsFillSuccess)
						},
							new Svg(Fxs.Symbols.Plus)
						),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsSidebarCreateLabel, Classes.FxsSidebarShowIfExpanded,
									Classes.FxsTrimText)
						},
							Text.CreateText
						)
					)
				)
			);
		}

		private ReactElement RenderSidebarMiddle()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsSidebarMiddle)
			},
				DOM.UL(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarFavorites)
				},
					props.Favorites.Select(RenderFavorite)
				)
			);
		}

		private ReactElement RenderFavorite(SideBarButton fav, uint index)
		{
			return DOM.Li(new LIAttributes
			{
				ClassName = Fluent.ClassName(Classes.FxsSidebarItem, Classes.FxsTrimHover, DummyClasses.FxsSidebarDraggable,
						Classes.FxsTrimBorder),
				//TODO: draggable=true
			},
				DOM.A(new AnchorAttributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarItemLink, Classes.FxsTrimText),
					Title = fav.Label
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarIcon,
								Classes.FxsTrimSvg)
					},
						fav.Icon
					),
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarLabel,
								Classes.FxsSidebarShowIfExpanded)
					},
						fav.Label
					),
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarExternal,
							Classes.FxsSidebarShowIfExpanded)
					}),
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarHandle,
								Classes.FxsTrimSvgSecondary)
					},
						new Svg(Fxs.Symbols.Ellipsis)
					)
				)
			);
		}

		private ReactElement RenderSidebarBottom()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsSidebarBottom)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarBrowse, Classes.FxsTrimHover, DummyClasses.FxsMenuBrowse),
					Title = Text.BrowseText
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSidebarButtonFlex)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsSidebarBrowseLabel, Classes.FxsTrimText,
									Classes.FxsSidebarShowIfExpanded)
						},
							Text.BrowseText
						),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsSidebarBrowseIcon, Classes.FxsTrimSvg)
						},
							new Svg(Fxs.Symbols.CaretUp)
						)
					)
				),
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsSidebarFlyout, Classes.FxsPopup, Classes.FxsPortalBgTxtBr)
							.Add(state.FlyoutIsHidden ? Classes.FxsSidebarFlyoutIsHidden : DummyClasses.FxsSidebarFlyoutIsOpen)
							.AddIf(_ => state.FlyoutIsHidden, DummyClasses.FxsSidebarBrowseShown)
				},
					new Svg(Fxs.Symbols.CaretUp)
				)
			);
		}

		public sealed class SideBarButton : IAmImmutable
		{
			public SideBarButton(string title, Action action, bool opensExternal, Svg icon)
			{
				this.CtorSet(_ => _.Label, title);
				this.CtorSet(_ => _.Action, action);
				this.CtorSet(_ => _.OpensExternal, opensExternal);
				this.CtorSet(_ => _.Icon, icon);
			}

			public string Label { get; }
			public Action Action { get; }
			public bool OpensExternal { get; }
			public Svg Icon { get; }
		}

		protected override State GetInitialState()
		{
			return new State(true, true);
		}

		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs, NonNullList<SideBarButton> favorites, NonNullList<SideBarButton> buttons)
			{
				this.CtorSet(_ => _.Fxs, fxs);
				this.CtorSet(_ => _.Favorites, favorites);
				this.CtorSet(_ => _.Buttons, buttons);
			}

			public Fxs Fxs { get; }
			public NonNullList<SideBarButton> Favorites { get; }
			public NonNullList<SideBarButton> Buttons { get; }
		}

		public sealed class State : IAmImmutable
		{
			public State(bool collapsed, bool flyoutIsHidden)
			{
				this.CtorSet(_ => _.Collapsed, collapsed);
				this.CtorSet(_ => _.FlyoutIsHidden, flyoutIsHidden);
			}

			public bool Collapsed { get; }
			public bool FlyoutIsHidden { get; }
		}
	}
}