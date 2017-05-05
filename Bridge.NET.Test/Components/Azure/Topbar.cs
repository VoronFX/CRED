using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public sealed class Topbar : PureComponent<Topbar.Props>
	{
		public Topbar(Fxs fxs, bool showStartboard, bool showPreview)
			: base(new Props(fxs, showStartboard, showPreview))
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.Topbar)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.TopbarExitCustomize)
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.TopbarExitCustomizeMessage)
					},
						Text.ExitCustomizeText
					),
					DOM.Button(new ButtonAttributes
					{
						ClassName = Fluent.ClassName(Classes.TopbarExitCustomizeButton, Classes.Button, Classes.PortalButtonPrimary)
					},
						Text.ExitCustomizeButton
					)
				),
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.TopbarContent, Classes.HideInCustomize)
				},
					props.ShowPreview
						? DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.TopbarInternal, Classes.BgWarning),
						},
							Text.InternalText
						)
						: null,
					DOM.A(new AnchorAttributes
					{
						ClassName = Fluent.ClassName(Classes.TopbarHome, Classes.TrimTextPrimary, Classes.TrimHover),
						Title = Text.DashboardTooltip,
						Href = "#"
					},
						Text.ProductName
					),
					RenderBreadcrumb(),
					//TODO: inDevMode
					//TODO: canReportBug
					new TopbarSearch(props.Fxs, false, false)
				)
			);
		}

		private ReactElement RenderBreadcrumb()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.Breadcrumb)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.BreadcrumbWrapper)
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.BreadcrumbDropmenu)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.Dropmenu)
						},
							DOM.Button(new ButtonAttributes
							{
								ClassName = Fluent.ClassName(Classes.DropmenuButton, Classes.PopupButton)
							},
								"«"
							),
							DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.DropmenuContent, Classes.TextLink, Classes.Popup, Classes.PortalBgTxtBr,
										Classes.Workaround.DropmenuDefaultWidth, Classes.DropmenuRight, Classes.DropmenuInvisible)
							},
								DOM.UL(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.BreadcrumbOverflow)
								}
								)
							)
						)
					)
				)
			);
		}

		private FxsClasses Classes => props.Fxs.Classes;
		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs, bool showStartboard, bool showPreview)
			{
				this.CtorSet(_ => _.Fxs, fxs);
				this.CtorSet(_ => _.ShowStartboard, showStartboard);
				this.CtorSet(_ => _.ShowPreview, showPreview);
			}

			public Fxs Fxs { get; }
			public bool ShowStartboard { get; }
			public bool ShowPreview { get; }
		}
	}
}