using System;
using AzurePortal;
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
							ClassName = Fluent.ClassName(Classes.FxsSearchbox, DummyClasses.FxsTopbarInput, Classes.FxcBase)
						}
						)
					)
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