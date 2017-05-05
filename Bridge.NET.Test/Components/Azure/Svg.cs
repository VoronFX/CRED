using Bridge.NET.Test.Components.Azure.Resources;
using Bridge.NET.Test.Helpers;
using Bridge.React;
using ProductiveRage.Immutable;

namespace Bridge.NET.Test.Components.Azure
{
	public class Svg : PureComponent<Svg.Props>
	{
		public Svg(Fxs.Symbols symbol)
			: base(new Props(symbol))
		{
		}

		public override ReactElement Render()
		{
			return DOMEx.Create(TagNames.Svg, new SvgAttributes
			{
				Style = new ReactStyle { Width = "100%", Height = "100%" }
			},
				DOMEx.Create(TagNames.Use, new UseAttributes
				{
					Href = props.Symbol.ToHref()
				})
			);
		}

		public class Props : IAmImmutable
		{
			public Props(Fxs.Symbols symbol)
			{
				this.CtorSet(_ => _.Symbol, symbol);
			}

			public Fxs.Symbols Symbol { get; }
		}
	}
}