using System.Linq;
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
			});
			return DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbar)
				},
				Fluent.ChildrenBuilder()
					.Add(
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomize)
							},
							Fluent.ChildrenBuilder()
								.Add(
									DOM.Div(new Attributes
										{
											ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomizeMessage)
										},
										Text.ExitCustomizeText
									))
								.Add(
									DOM.Button(new ButtonAttributes
										{
											ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomizeButton, Classes.FxsButton,
												Classes.FxsPortalButtonPrimary)
										},
										Text.ExitCustomizeButton
									))
								.Build()
						))
				//	.Add(RenderContent())
					.Add(
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsTopbarDashboard)
							},
							Fluent.ChildrenBuilder()
								.Add(
									DOM.Div(new Attributes
										{
											ClassName = Fluent.ClassName(Classes.FxsTopbarDashboardMessage)
										},
										Text.DashboardMessage
									))
								.Add(RenderDashboardButton(Text.ExitCustomizeButton,
									Fluent.ClassName(DummyClasses.FxsTopbarDashboardDoneCustomize, Classes.FxsDisplayNone)))
								.Add(RenderDashboardButton(Text.ViewDashboardButton,
									Fluent.ClassName(Classes.FxsTopbarDashboardView, Classes.FxsPortalButtonPrimary)))
								.Add(RenderDashboardButton(Text.DashboardSaveButtonText,
									Fluent.ClassName(Classes.FxsTopbarDashboardSave, Classes.FxsButtonDefault)))
								.Add(RenderDashboardButton(Text.DiscardDashboardButton,
									Fluent.ClassName(Classes.FxsTopbarDashboardDiscard, Classes.FxsButtonDefault)))
								.Build()
						))
					.Build()
			);
		}

		private ReactElement RenderContent()
		{
			return DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbarContent, Classes.FxsHideInCustomize)
				},
				Fluent.ChildrenBuilder()
					.FluentIf(_ => props.ShowPreview, _ =>
						_.Add(
							DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.FxsTopbarInternal, Classes.FxsBgWarning),
								},
								Text.InternalText
							)))
					.Add(
						DOM.A(new AnchorAttributes
							{
								ClassName = Fluent.ClassName(Classes.FxsTopbarHome, Classes.FxsTrimTextPrimary, Classes.FxsTrimHover),
								Title = Text.DashboardTooltip,
								Href = "#"
							},
							Text.ProductName
						))

					//.Add(new Breadcrumb(props.fxs, ),)
					//TODO: inDevMode
					//TODO: canReportBug
					.Add(new TopbarSearch(props.Fxs, false, false))
					.Add(
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsTopbarNotifications, Classes.FxsTopbarButton, Classes.FxsTrimHover,
									Classes.FxsTrimSvg),
								Title = Text.Notifications,
							},
							DOM.Div(new Attributes(),
								new Svg(Fxs.Symbols.NotificationsIcon)
							),
							DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.FxsNotificationspaneProgressbar, Classes.FxsDisplayNone)
								},
								DOM.Div(new Attributes())
							)
						))
					.Add(
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(DummyClasses.FxsTopbarToast)
							},
							DOM.UL(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsToast),
								// DataBind = visible: data.shouldShowToasts, foreach: func._uiItems,
							})
						))
					.Add(RenderTopbarButton(Text.Console,
						Fluent.ClassName(Classes.FxsConsole), Fxs.Symbols.CaretUp))
					.Add(RenderTopbarButton(Text.Settings,
						Fluent.ClassName(DummyClasses.FxsTopbarSettings, DummyClasses.FxsMenuSettings), Fxs.Symbols.SettingsIcon))
					.Add(RenderTopbarButton(Text.Feedback,
						Fluent.ClassName(DummyClasses.FxsTopbarFeedback), Fxs.Symbols.FeedbackIcon))
					.Add(RenderTopbarButton(Text.HelpTooltip,
						Fluent.ClassName(DummyClasses.FxsTopbarHelpmenu), Fxs.Symbols.HelpIcon))
					.Add(new AvatarMenu(props.Fxs))
					.Build()
			);
		}

		private ReactElement RenderTopbarButton(string title, Fluent.FluentClassName classes, Fxs.Symbols icon)
			=> DOM.A(new AnchorAttributes
			{
				ClassName = classes.Add(
					Classes.FxsTopbarButton,
					Classes.FxsTrimHover,
					Classes.FxsTrimSvg),
				Title = title
			}, new Svg(icon));

		private ReactElement RenderDashboardButton(string title, Fluent.FluentClassName classes)
			=> DOM.Button(new ButtonAttributes
			{
				ClassName = classes.Add(Classes.FxsButton),
				Title = title
			});


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