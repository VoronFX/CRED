using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Namespace;
using CsCodeGenerator.Interfaces;

namespace CsCodeGenerator
{
	namespace Types
	{
		internal struct EmptyLine : IEmptyLine
		{
			private static readonly string[] EmptyLineArray =
				{string.Empty};

			public IEnumerable<string> Build() => EmptyLineArray;
		}

		internal class Namespace : INamespace
		{
			private readonly IEnumerable<INamespaceElement> content;

			public string Name { get; }

			INamespaceElement[] INamespace.Elements => content.ToArray();

			public Namespace(string name, IEnumerable<INamespaceElement> content)
			{
				this.content = content;
				Name = name;
			}

			public IEnumerable<string> Build() =>
				new[] { $"namespace {Name}", "{" }
					.Concat(content.SelectMany(x => x.Build()))
					.Concat(new[] { "}" });
		}

		internal class Using : Content, IUsing
		{
			public Using(IEnumerable<string> content)
				: base(content.Select(x => $"using {x};"))
			{
			}
		}

		internal class SummaryComment : Content, IComment
		{
			public SummaryComment(IEnumerable<string> content)
				: base(new[] { "/// <summary>" }
					.Concat(content.Select(x => "/// " + new XElement("dummy", x).Value))
					.Concat(new[] { "/// </summary>" }))
			{
			}
		}

		internal class Comment : Content, IComment
		{
			public Comment(IEnumerable<string> content)
				: base(content.Select(x => "// " + x))
			{
			}
		}

		internal class Document : Content, IDocument
		{
			public Document(
				IEnumerable<IDocumentHeadElement> headContent, 
				IEnumerable<IDocumentBodyElement> bodyElements): 
				base(headContent.Cast<IElement>().Concat(bodyElements).SelectMany(x => x.Build()))
			{ }

			public void Save(string path) =>
				File.WriteAllLines(path, Build());
		}

		internal class Content : IElement
		{
			private readonly IEnumerable<string> content;

			public Content(IEnumerable<string> content)
			{
				this.content = content;
			}

			public IEnumerable<string> Build() => content;
		}
	}
}
