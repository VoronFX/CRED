using System;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public sealed class SideBar : Component<SideBar.Props, SideBar.State>
	{
		public SideBar(Fxs fxs, NonNullList<SideBarButton> favorites, NonNullList<SideBarButton> buttons)
			: base(new Props(fxs, favorites, buttons))
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(
				new Attributes
				{
					ClassName = Fluent.ClassName(Classes.Sidebar, Classes.TrimBorder)
						.AddIf(() => state.Collapsed, Classes.Sidebar)
				},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarBar, Classes.Trim, Classes.TrimText)
				},
					RenderSidebarTop(),
					RenderSidebarMiddle(),
					RenderSidebarBottom()
				));
		}

		private ReactElement RenderSidebarTop()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.SidebarTop)
			},
				DOM.Button(new ButtonAttributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarCollapseButton, Classes.TrimSvg),
					Title = Text.ShowMenu,
					OnClick = e => SetState(state.With(_ => _.Collapsed, !state.Collapsed))
				},
					new Svg(Fxs.Symbols.Hamburger)
				),
				DOM.Button(new ButtonAttributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarCreate, Classes.TrimHover, Classes.TrimBorder),
					Title = Text.CreateText,
					//OnClick = e => SetState(state.With(_ => _.Collapsed, !state.Collapsed))
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.SidebarButtonFlex)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.SidebarCreateIcon, Classes.FillSuccess)
						},
							new Svg(Fxs.Symbols.Plus)
						)
					),
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.SidebarCreateLabel, Classes.SidebarShowIfExpanded, Classes.TrimText)
					},
						Text.CreateText
					)
				)
			);
		}

		private ReactElement RenderSidebarMiddle()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.SidebarMiddle)
			},
				DOM.UL(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarFavorites)
				},
					props.Favorites.Select((fav, index) =>
						DOM.Li(new LIAttributes
						{
							ClassName = Fluent.ClassName(Classes.SidebarItem, Classes.TrimHover, Classes.SidebarDraggable,
									Classes.SidebarDraggable,
									Classes.TrimBorder),
							//TODO: draggable=true
						},
							DOM.A(new AnchorAttributes
							{
								ClassName = Fluent.ClassName(Classes.SidebarItemLink, Classes.TrimText),
								Title = fav.Label
							},
								DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.SidebarIcon, Classes.TrimSvg)
								},
									fav.Icon
								),
								DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.SidebarLabel, Classes.SidebarShowIfExpanded)
								},
									fav.Label
								),
								DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.SidebarExternal, Classes.SidebarShowIfExpanded)
								}
								),
								DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.SidebarHandle, Classes.TrimSvgSecondary)
								},
									new Svg(Fxs.Symbols.Ellipsis)
								)
							)
						)))
			);
		}

		private ReactElement RenderSidebarBottom()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.SidebarBottom)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarBrowse, Classes.TrimHover, Classes.MenuBrowse),
					Title = Text.BrowseText
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.SidebarButtonFlex)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.SidebarBrowseLabel, Classes.TrimText, Classes.SidebarShowIfExpanded)
						},
							Text.BrowseText
						),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.SidebarBrowseIcon, Classes.TrimSvg)
						},
							new Svg(Fxs.Symbols.CaretUp)
						)
					)
				),
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.SidebarFlyout, Classes.Popup, Classes.PortalBgTxtBr)
						.Add(state.FlyoutIsHidden ? Classes.SidebarFlyoutIsHidden : Classes.SidebarFlyoutIsOpen)
						.AddIf(() => state.FlyoutIsHidden, Classes.SidebarBrowseShown)
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

		private FxsClasses Classes => props.Fxs.Classes;
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