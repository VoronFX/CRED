using System.Collections.Generic;
using Bridge.Html5;
using Bridge.React;

namespace Bridge.NET.Test.Helpers
{
	[Name("React.DOMEx")]
	[External]
	public static class DOMEx
	{
		//[Name("a")]
		[Template("React.createElement({0}, {1}, System.Linq.Enumerable.from({2}).toArray())")]
		public static extern ReactElement Create(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			params Union<ReactElement, string>[] children);

		[Template("React.createElement({0}, null, System.Linq.Enumerable.from({1}).toArray())")]
		public static extern ReactElement Create(TagNames tagName, IEnumerable<ReactElement> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, null, Bridge.React.toReactElementArray({1}))")]
		public static extern ReactElement Create<TProps>(TagNames tagName, IEnumerable<PureComponent<TProps>> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, null, Bridge.React.toReactElementArray({1}))")]
		public static extern ReactElement Create<TProps>(TagNames tagName, IEnumerable<StatelessComponent<TProps>> children);

		[Template("React.createElement({0}, null, System.Linq.Enumerable.from({1}).toArray())")]
		public static extern ReactElement Create(TagNames tagName, IEnumerable<string> children);

		[Template("React.createElement({0}, null, {1})")]
		public static extern ReactElement Create(TagNames tagName, ReactElement child);

		[Template("React.createElement({0}, null, {1})")]
		public static extern ReactElement Create(TagNames tagName, string child);

		[Template("React.createElement({0}, {1}, System.Linq.Enumerable.from({2}).toArray())")]
		public static extern ReactElement Create(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			IEnumerable<ReactElement> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, {1}, Bridge.React.toReactElementArray({2}))")]
		public static extern ReactElement Create<TProps>(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			IEnumerable<PureComponent<TProps>> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, {1}, Bridge.React.toReactElementArray({2}))")]
		public static extern ReactElement Create<TProps>(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			IEnumerable<StatelessComponent<TProps>> children);

		[Template("React.createElement({0}, {1}, System.Linq.Enumerable.from({2}).toArray())")]
		public static extern ReactElement Create(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			IEnumerable<string> children);

		[Template("React.createElement({0}, {1}, {2})")]
		public static extern ReactElement Create(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			ReactElement child);

		[Template("React.createElement({0}, {1}, {2})")]
		public static extern ReactElement Create(TagNames tagName, ReactDomElementAttributes<HTMLElement> properties,
			string child);

		//public static ReactElement Svg(SvgAttributes properties, params Union<ReactElement, string>[] children)
		//	=> Create(TagNames.Svg, properties, children);

		//public static ReactElement Svg(IEnumerable<ReactElement> children)
		//	=> Create(TagNames.Svg, children);

		//public static ReactElement Svg<TProps>(IEnumerable<PureComponent<TProps>> children)
		//	=> Create(TagNames.Svg, children);

		//public static ReactElement Svg<TProps>(IEnumerable<StatelessComponent<TProps>> children)
		//	=> Create(TagNames.Svg, children);

		//public static ReactElement Svg(IEnumerable<string> children)
		//	=> Create(TagNames.Svg, children);

		//public static ReactElement Svg(ReactElement child)
		//	=> Create(TagNames.Svg, child);

		//public static ReactElement Svg(string child)
		//	=> Create(TagNames.Svg, child);

		//public static ReactElement Svg(SvgAttributes properties, IEnumerable<ReactElement> children)
		//	=> Create(TagNames.Svg, properties, children);

		//public static ReactElement Svg<TProps>(SvgAttributes properties, IEnumerable<PureComponent<TProps>> children)
		//	=> Create(TagNames.Svg, properties, children);

		//public static ReactElement Svg<TProps>(SvgAttributes properties, IEnumerable<StatelessComponent<TProps>> children)
		//	=> Create(TagNames.Svg, properties, children);

		//public static ReactElement Svg(SvgAttributes properties, IEnumerable<string> children)
		//	=> Create(TagNames.Svg, properties, children);

		//public static ReactElement Svg(SvgAttributes properties, ReactElement child)
		//	=> Create(TagNames.Svg, properties, child);

		//public static ReactElement Svg(SvgAttributes properties, string child)
		//	=> Create(TagNames.Svg, properties, child);
	}

	[External]
	[Enum(Emit.StringNameLowerCase)]
	[Name("String")]
	public enum TagNames
	{
		Svg,
		Use
	}

	//[External]
	//[Name("SVGElement")]
	//public class SVGElement : HTMLElement<SVGElement>
	//{
	//	[Name("role")]
	//	public RoleType Role;

	//	[Template("document.createElement(\'svg\')")]
	//	public extern SVGElement();
	//}


	[External]
	[ObjectLiteral]
	public sealed class SvgAttributes : ReactDomElementAttributes<HTMLElement>
	{
		[Name("role")]
		public RoleType Role { private get; set; }
	}

	[External]
	[ObjectLiteral]
	public sealed class UseAttributes : ReactDomElementAttributes<HTMLElement>
	{
		[Name("href")]
		public string Href { private get; set; }
	}

	[External]
	[Enum(Emit.StringNameLowerCase)]
	[Name("String")]
	public enum RoleType
	{
		Presentation,
		Button,
		MenuItem,
		Link
	}
}