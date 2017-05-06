using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AzurePortalExtractor
{
	public static class HtmlToReactConverter
	{
		public static string ToPascalCase(this string text)
			=> text.ToUpperInvariant().Substring(0, 1) + text.Substring(1);

		public static IEnumerable<string> CreateComment(string text)
			=> (string.IsNullOrWhiteSpace(text) || text == "$1")
				? new string[] { }
				: text
					.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
					.Select(s => $"// {s}");

		public static IEnumerable<string> CreateHead(HtmlNode node)
			=> new[]
				{
					!node.Attributes.Contains("class")
						? string.Empty
						: $"ClassName = Fluent.ClassName(" + string.Join(", ",
							  node.Attributes["class"]
								  .Value
								  .Split(' ')
								  .Select(c => "Classes."+Regex.Replace(
									  Regex.Replace("-" + c, "(?si)[^A-Za-z0-9]+", "-"), "(?si)-[A-Za-z0-9]",
									  x => x.Value
										  .ToUpperInvariant()
										  .Substring(1)))) + "),"
				}
				.Concat(node
					.Attributes
					.Where(a => a.Name != "class")
					.Select(a => $"// {a.Name.ToPascalCase()} = {a.Value},"))
				.Concat(CreateComment(node.OuterHtml.Substring(0,
					node.OuterHtml.IndexOf(node.InnerHtml, StringComparison.Ordinal))));

		public static IEnumerable<string> CreateElement(HtmlNode node)
			=> (node.NodeType == HtmlNodeType.Text || node.NodeType == HtmlNodeType.Comment)
				? CreateComment(node.OuterHtml)
				: new string[] { }
					.Concat(new[]
					{
						$"DOM.{node.Name.ToPascalCase()}(new Attributes",
						"{"
					})
					.Concat(CreateHead(node).Indent())
					.Concat(new[]
					{
						"}),",
					})
					.Concat(node.ChildNodes.SelectMany(CreateElement).Indent())
					.Concat(new[]
					{
						"),"
					});
	}
}