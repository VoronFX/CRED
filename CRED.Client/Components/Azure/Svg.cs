using Bridge.React;
using CRED.Client.Components.Azure.Resources;
using CRED.Client.Helpers;
using ProductiveRage.Immutable;

namespace CRED.Client.Components.Azure
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
				Width = "100%",
				Height = "100%"
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