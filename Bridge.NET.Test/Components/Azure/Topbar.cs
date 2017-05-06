using AzurePortal;
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
				ClassName = Fluent.ClassName(Classes.FxsTopbar)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomize)
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomizeMessage)
					},
						Text.ExitCustomizeText
					),
					DOM.Button(new ButtonAttributes
					{
						ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomizeButton, Classes.FxsButton, Classes.FxsPortalButtonPrimary)
					},
						Text.ExitCustomizeButton
					)
				),
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbarContent, Classes.FxsHideInCustomize)
				},
					props.ShowPreview
						? DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsTopbarInternal, Classes.FxsBgWarning),
						},
							Text.InternalText
						)
						: null,
					DOM.A(new AnchorAttributes
					{
						ClassName = Fluent.ClassName(Classes.FxsTopbarHome, Classes.FxsTrimTextPrimary, Classes.FxsTrimHover),
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
				ClassName = Fluent.ClassName(Classes.FxsBreadcrumb)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsBreadcrumbWrapper)
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsBreadcrumbDropmenu)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsDropmenu)
						},
							DOM.Button(new ButtonAttributes
							{
								ClassName = Fluent.ClassName(Classes.FxsDropmenuButton, DummyClasses.FxsPopupButton)
							},
								"«"
							),
							DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsDropmenuContent, DummyClasses.FxsTextLink, Classes.FxsPopup, Classes.FxsPortalBgTxtBr,
										Classes.FxsDropmenuDefaultWidth, Classes.FxsDropmenuRight, Classes.FxsDropmenuInvisible)
							},
								DOM.UL(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.FxsBreadcrumbOverflow)
								}
								)
							)
						)
					)
				)
			);
		}


		private StyleClassesMap Classes => props.Fxs.StyleClasses;
		private DummyClassesMap DummyClasses => props.Fxs.DummyClasses;
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