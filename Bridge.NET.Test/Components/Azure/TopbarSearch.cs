using System;
using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;
using CRED;

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
				ClassName = Fluent.ClassName(Classes.TopbarSearch, Classes.Search),
				Title = Text.SearchTooltip

			},
				DOM.Div(new Attributes
				{
					ClassName = Fluent.ClassName(Classes.Search)
							.AddIf(() => state.IsExpanded, Classes.SearchExpanded)
							.AddIf(() => !props.JourneysShown, Classes.SearchNoJourneys)
				},
					DOM.Div(new Attributes
					{
						ClassName = Fluent.ClassName(Classes.BoxWrapper)
								.AddIf(() => props.ShowSearching, Classes.Searching)
					},
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.Icon, Classes.Workaround.MsPortalFxSvgFlipHorizontal)
						},
							new Svg(Fxs.Symbols.Search)
						),
						DOM.Div(new Attributes
						{
							ClassName = Fluent.ClassName(Classes.Searchbox, Classes.TopbarInput, Classes.Workaround.Base)
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

		private FxsClasses Classes => props.Fxs.Classes;
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