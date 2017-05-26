using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace CsCodeGenerator
{
	public static class Generator
	{
		public static IEnumerable<string> Namespace(string name, string[] comment, IEnumerable<string> imports, IEnumerable<string> content)
			=> imports.Select(i => $"using {i};")
				.Concat(new[] { string.Empty })
				.Concat(comment.Select(x => $@"// {x}"))
				.Concat(new[]
				{
					"namespace "+name,
					"{",
				})
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}"
				});

		public static IEnumerable<string> Class(string name, string[] comment, IEnumerable<string> content,
			bool @static, bool partial, bool @public = true, bool @sealed = true)
			=> Comment(comment)
				.Concat(new[]
				{
					JoinNonEmpty(" ",
						@public ? nameof(@public) : string.Empty,
						@sealed ? nameof(@sealed) : string.Empty,
						@static ? nameof(@static) : string.Empty,
						partial ? nameof(partial) : string.Empty, "class", name),
					"{"
				})
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}",
					string.Empty
				});

		public static IEnumerable<string> Property(string name, string value, string[] comment,
			bool @const, bool @static, bool field, bool @public = true, bool @readonly = true, string type = "string")
			=> Comment(comment)
				.Concat(new[]
				{
					JoinNonEmpty(" ",
						@public ? nameof(@public) : string.Empty,
						@static ? nameof(@static) : string.Empty,
						@const ? nameof(@const) : string.Empty,
						@readonly && field ? nameof(@readonly) : string.Empty,
						type, name,
						field ? "=" : (@readonly? "{ get; } =" :"{ get; set; } ="), value+";"),
					string.Empty
				});

		public static IEnumerable<string> Comment(string[] comment) =>
			comment.Any() ?
				new[] { "/// <summary>" }

					.Concat(comment.Select(c => "/// " + new XElement("dummy", c).Value))
					.Concat(new[] { "/// </summary>" }) : new string[] { };

		public static IEnumerable<string> Indent(this IEnumerable<string> input)
			=> input.Select(x => "    " + x);

		private static string JoinNonEmpty(string separator, params string[] strings)
			=> string.Join(separator, strings.Where(s => !string.IsNullOrWhiteSpace(s)));
	}
}
