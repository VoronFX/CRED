using System.Collections.Generic;
using System.Linq;
using AzurePortal;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public sealed class Breadcrumb : PureComponent<Breadcrumb.Props>
	{
		public Breadcrumb(Fxs fxs, NonNullList<Blade> crumbs)
			: base(new Props(fxs, crumbs))
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsBreadcrumb)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsBreadcrumbWrapper)
				},
					Fluent.ChildrenBuilder()
						.Add(
							RenderDropmenu())
						.AddRange(
							RenderCrumbs())
						.Build()
				)
			);
		}

		private ReactElement RenderDropmenu()
		{
			return DOM.Div(new Attributes
			{
				ClassName = Fluent.ClassName(Classes.FxsBreadcrumbDropmenu)
			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsDropmenu)
				},
					Fluent.ChildrenBuilder()
						.Button(_=>
							{
								_.ClassName = Fluent.ClassName(Classes.FxsDropmenuButton, DummyClasses.FxsPopupButton)
							},
								"«"
							))
						.Div(
							DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsDropmenuContent, DummyClasses.FxsTextLink, Classes.FxsPopup,
										Classes.FxsPortalBgTxtBr,
										Classes.FxsDropmenuDefaultWidth, Classes.FxsDropmenuRight, Classes.FxsDropmenuInvisible)
							},
								DOM.UL(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.FxsBreadcrumbOverflow)
								})
							))
						.Build()
				)
			);
		}

		private IEnumerable<ReactElement> RenderCrumbs()
		{
			return props.Crumbs
				.Where(crumb => crumb.Visible)
				.SelectMany((crumb, i) => Fluent.ChildrenBuilder()
					.Add(
						DOM.A(new AnchorAttributes
						{
							ClassName = Fluent.ClassName(Classes.FxsBreadcrumbCrumb, Classes.FxsTrimText, Classes.FxsTrimHover),
							// Href = ,
							// DataBind = attr: { 'data-blade-id': $data.bladeId }, text: $data.bladeTitle, visible: $data.visible,
							// Style = display: none;,
						}))
					.FluentIf(_ => i < props.Crumbs.Count, _ =>
						_.Add(
							DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsBreadcrumbDivider, Classes.FxsTrimSvgSecondary),
								// DataBind = image: $data.caretUp, visible: $data.visible,
								// Style = display: none;,
								// <div class="fxs-breadcrumb-divider fxs-trim-svg-secondary" data-bind="image: $data.caretUp, visible: $data.visible" style="display: none;">
							},
								new Svg(Fxs.Symbols.CaretUp)
							))
					)
					.Build()
				);
		}

		private StyleClassesMap Classes => props.Fxs.StyleClasses;
		private DummyClassesMap DummyClasses => props.Fxs.DummyClasses;
		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs, NonNullList<Blade> crumbs)
			{
				this.CtorSet(_ => _.Fxs, fxs);
				this.CtorSet(_ => _.Crumbs, crumbs);
			}

			public Fxs Fxs { get; }
			public NonNullList<Blade> Crumbs { get; }
		}
	}

	public sealed class Blade : IAmImmutable
	{
		public Blade(bool visible, string title)
		{
			this.CtorSet(_ => _.Visible, visible);
			this.CtorSet(_ => _.Title, title);
		}

		public bool Visible { get; }
		public string Title { get; }
	}
}