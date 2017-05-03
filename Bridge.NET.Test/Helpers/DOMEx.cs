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
		public static extern ReactElement Create(string tagName, DomElementsAttributes properties,
			params Union<ReactElement, string>[] children);

		[Template("React.createElement({0}, null, System.Linq.Enumerable.from({1}).toArray())")]
		public static extern ReactElement Create(string tagName, IEnumerable<ReactElement> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, null, Bridge.React.toReactElementArray({1}))")]
		public static extern ReactElement Create<TProps>(string tagName, IEnumerable<PureComponent<TProps>> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, null, Bridge.React.toReactElementArray({1}))")]
		public static extern ReactElement Create<TProps>(string tagName, IEnumerable<StatelessComponent<TProps>> children);

		[Template("React.createElement({0}, null, System.Linq.Enumerable.from({1}).toArray())")]
		public static extern ReactElement Create(string tagName, IEnumerable<string> children);

		[Template("React.createElement({0}, null, {1})")]
		public static extern ReactElement Create(string tagName, ReactElement child);

		[Template("React.createElement({0}, null, {1})")]
		public static extern ReactElement Create(string tagName, string child);

		[Template("React.createElement({0}, {1}, System.Linq.Enumerable.from({2}).toArray())")]
		public static extern ReactElement Create(string tagName, DomElementsAttributes properties,
			IEnumerable<ReactElement> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, {1}, Bridge.React.toReactElementArray({2}))")]
		public static extern ReactElement Create<TProps>(string tagName, DomElementsAttributes properties,
			IEnumerable<PureComponent<TProps>> children);

		[IgnoreGeneric]
		[Template("React.createElement({0}, {1}, Bridge.React.toReactElementArray({2}))")]
		public static extern ReactElement Create<TProps>(string tagName, DomElementsAttributes properties,
			IEnumerable<StatelessComponent<TProps>> children);

		[Template("React.createElement({0}, {1}, System.Linq.Enumerable.from({2}).toArray())")]
		public static extern ReactElement Create(string tagName, DomElementsAttributes properties,
			IEnumerable<string> children);

		[Template("React.createElement({0}, {1}, {2})")]
		public static extern ReactElement Create(string tagName, DomElementsAttributes properties,
			ReactElement child);

		[Template("React.createElement({0}, {1}, {2})")]
		public static extern ReactElement Create(string tagName, DomElementsAttributes properties,
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

	public static class DOMEx2
	{

		public static ReactElement Create<TAttr>(TAttr properties, params Union<ReactElement, string>[] children)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, children);

		public static ReactElement Create<TAttr>(TAttr properties, IEnumerable<ReactElement> children)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, children);

		public static ReactElement Svg<TAttr, TProps>(TAttr properties, IEnumerable<PureComponent<TProps>> children)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, children);

		public static ReactElement Svg<TAttr, TProps>(TAttr properties, IEnumerable<StatelessComponent<TProps>> children)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, children);

		public static ReactElement Create<TAttr>(TAttr properties, IEnumerable<string> children)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, children);

		public static ReactElement Create<TAttr>(TAttr properties, ReactElement child)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, child);

		public static ReactElement Create<TAttr>(TAttr properties, string child)
			where TAttr : TagNameAttributes
			=> DOMEx.Create(properties.TagName.ToString(), properties, child);

	}

	[Enum(Emit.StringNameLowerCase)]
	[Name("String")]
	public enum TagNames
	{
		Svg
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

	public abstract class TagNameAttributes : ReactDomElementAttributes<HTMLElement>
	{
		public TagNames TagName { get; }

		protected TagNameAttributes(TagNames tag)
		{
			TagName = tag;
		}
	}

	[External]
	[ObjectLiteral]
	public class SvgAttributes : TagNameAttributes
	{
		[Name("role")]
		public RoleType Role { private get; set; }

		public SvgAttributes() 
			: base(TagNames.Svg) { }
	}

	[External]
	[Enum(Emit.StringNameLowerCase)]
	[Name("String")]
	public enum RoleType
	{
		Presentation,
		Button,
	}
}