using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using CRED.Client.Helpers;
using ProductiveRage.Immutable;
using Classes = CRED.Client.AzureCssClassesMap;
using DummyClasses = CRED.Client.AzureCssMissingClassesMap;


namespace CRED.Client.Components.Azure
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
							ClassName = Fluent.ClassName(Classes.FxsTopbarExitCustomizeButton, Classes.FxsButton,
								Classes.FxsPortalButtonPrimary)
						},
						Text.ExitCustomizeButton
					)
				),
				RenderContent(),
				DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsTopbarDashboard)
					},
					DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsTopbarDashboardMessage)
						},
						Text.DashboardMessage
					),
					RenderDashboardButton(Text.ExitCustomizeButton,
						Fluent.ClassName(DummyClasses.FxsTopbarDashboardDoneCustomize, Classes.FxsDisplayNone)
					),
					RenderDashboardButton(Text.ViewDashboardButton,
						Fluent.ClassName(Classes.FxsTopbarDashboardView, Classes.FxsPortalButtonPrimary)
					),
					RenderDashboardButton(Text.DashboardSaveButtonText,
						Fluent.ClassName(Classes.FxsTopbarDashboardSave, Classes.FxsButtonDefault)
					)
				),
				RenderDashboardButton(Text.DiscardDashboardButton,
					Fluent.ClassName(Classes.FxsTopbarDashboardDiscard, Classes.FxsButtonDefault)
				)
			);
		}

		private ReactElement RenderContent()
		{
			return DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbarContent, Classes.FxsHideInCustomize)
				},
				!props.ShowPreview
					? null
					: DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsTopbarInternal, Classes.FxsBgWarning),
						},
						Text.InternalText
					),
				DOM.A(new AnchorAttributes
					{
						ClassName = Fluent.ClassName(Classes.FxsTopbarHome, Classes.FxsTrimTextPrimary, Classes.FxsTrimHover),
						Title = Text.DashboardTooltip,
						Href = "#"
					},
					Text.ProductName
				),

				//.Add(new Breadcrumb(props.fxs, ),)
				//TODO: inDevMode
				//TODO: canReportBug
				new TopbarSearch(props.Fxs, false, false),
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
				),
				DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(DummyClasses.FxsTopbarToast)
					},
					DOM.UL(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsToast)
						// DataBind = visible: data.shouldShowToasts, foreach: func._uiItems,
					})
				),
				RenderTopbarButton(Text.Console,
					Fluent.ClassName(Classes.FxsConsole),
					Fxs.Symbols.CaretUp
				),
				RenderTopbarButton(Text.Settings,
					Fluent.ClassName(DummyClasses.FxsTopbarSettings, DummyClasses.FxsMenuSettings),
					Fxs.Symbols.SettingsIcon
				),
				RenderTopbarButton(Text.Feedback,
					Fluent.ClassName(DummyClasses.FxsTopbarFeedback),
					Fxs.Symbols.FeedbackIcon
				),
				RenderTopbarButton(Text.HelpTooltip,
					Fluent.ClassName(DummyClasses.FxsTopbarHelpmenu),
					Fxs.Symbols.HelpIcon
				),
				new AvatarMenu(props.Fxs)
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