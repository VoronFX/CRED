using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class Svg : PureComponent<Svg.Props>
	{
		public Svg(FxSymbols symbol)
			: base(new Props(symbol)) { }

		public override ReactElement Render()
		{
			return DOMEx.Create(TagNames.Svg, new SvgAttributes
			{
				Style = new ReactStyle { Width = "100%", Height = "100%" }
			}, DOMEx.Create(TagNames.Use, new UseAttributes
			{
				Href = FxSymbols.Hamburger.ToHref()
			}));
		}

		public class Props : IAmImmutable
		{
			public Props(FxSymbols symbol)
			{
				this.CtorSet(_ => _.Symbol, symbol);
			}
			public FxSymbols Symbol { get; }
		}
	}


}