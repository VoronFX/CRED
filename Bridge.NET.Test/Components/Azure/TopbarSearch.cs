using System;
using AzurePortal;
using Bridge.Html5;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public sealed class TopbarSearch : Component<TopbarSearch.Props, TopbarSearch.State>
	{
		public TopbarSearch(Fxs fxs, bool journeysShown, bool showSearching)
			: base(new Props(fxs, journeysShown, showSearching))
		{
		}

		public override ReactElement Render()
		{
			return DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.FxsTopbarSearch, Classes.FxsSearch),
					Title = Text.SearchTooltip
				},
				DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.FxsSearch)
							.AddIf(() => state.IsExpanded, Classes.FxsSearchExpanded)
							.AddIf(() => !props.JourneysShown, Classes.FxsSearchNoJourneys)
					},
					DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.FxsBoxWrapper)
								.AddIf(() => props.ShowSearching, Classes.FxsSearching)
						},
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsIcon, Classes.MsportalfxSvgFlipHorizontal)
							},
							new Svg(Fxs.Symbols.Search)
						),
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsSearchbox, DummyClasses.FxsTopbarInput,
									Classes.FxcBase, Classes.AzcControl, DummyClasses.AzcEditableControl,
									Classes.AzcValidatableControl, Classes.AzcInputbox, Classes.AzcTextBox)
							},
							DOM.Div(new Attributes
								{
									ClassName = Fluent.ClassName(Classes.AzcInputboxWrapper, Classes.AzcTextBoxWrapper),
								},
								DOM.Input(new InputAttributes
								{
									ClassName = Fluent.ClassName(Classes.AzcInput, Classes.AzcFormControl, Classes.AzcValidationBorder),
									Type = InputType.Text,

									// DataBind = value: data.value, css: { &quot;fxs-br-error&quot;: data.validationState() === 1, &quot;fxs-br-dirty&quot;: data.dirty(), &quot;azc-disabled&quot;: $disabled, &quot;azc-br-focused&quot;: data.focused() }, valueUpdate: $ctl._getValueUpdateTrigger(), attr: { name: $ctl._name, placeholder: data.placeholder, readonly: data.readonly, spellcheck: $ctl._spellcheck, disabled: $disabled, tabindex: $tabIndex, &quot;aria-readonly&quot;: data.readonly, &quot;aria-disabled&quot;: $disabled },

									Name = "__azc-textBox0",
									Placeholder = "Поиск ресурсов",
									// Spellcheck = false,
									 TabIndex = 0,
								})
							)
						),
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsIndicator),
							},
							DOM.Div(new Attributes()),
							DOM.Div(new Attributes()),
							DOM.Div(new Attributes())
						),
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.FxsCross),
							},
							"×"
						)
					),
					DOM.A(new AnchorAttributes
						{
							ClassName = Fluent.ClassName(Classes.FxsSearchIcon, Classes.FxsTopbarButton, Classes.FxsTrimSvg,
								Classes.FxsTrimHover),
							Href = "#",
						},
						DOM.Div(new Attributes
							{
								ClassName = Fluent.ClassName(Classes.MsportalfxSvgFlipHorizontal),
							},
							new Svg(Fxs.Symbols.Search)
						)
					)
					//TODO: search widget
				)
			);
		}

		protected override State GetInitialState()
		{
			return new State(false);
		}

		private StyleClassesMap Classes => props.Fxs.StyleClasses;
		private DummyClassesMap DummyClasses => props.Fxs.DummyClasses;
		private IFxsText Text => props.Fxs.Text;

		public sealed class Props : IAmImmutable
		{
			public Props(Fxs fxs, bool journeysShown, bool showSearching)
			{
				this.CtorSet(_ => _.Fxs, fxs);
				this.CtorSet(_ => _.JourneysShown, journeysShown);
				this.CtorSet(_ => _.ShowSearching, showSearching);
			}

			public Fxs Fxs { get; }
			public bool JourneysShown { get; }
			public bool ShowSearching { get; }
		}

		public sealed class State : IAmImmutable
		{
			public State(bool isExpanded)
			{
				this.CtorSet(_ => _.IsExpanded, isExpanded);
			}

			public bool IsExpanded { get; }
		}
	}
}