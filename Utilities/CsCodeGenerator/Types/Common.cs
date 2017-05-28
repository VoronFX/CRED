using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Namespace;

namespace CsCodeGenerator.Types
{
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
		public Using(bool @static, string target)
			: base(new[] { "using " + (@static ? "static " : null) + target + ";" })
		{
		}

		public Using(string alias, string target)
			: base(new[] { $"using {alias} = {target};" })
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

	internal struct Document : IDocument, IDocumentWithHead
	{
		private readonly IEnumerable<string> docContent;

		public void Save(string path) =>
			File.WriteAllLines(path, Build());

		public Document(IEnumerable<string> content)
		{
			docContent = content;
		}

		public IDocumentWithHead Head(params IDocumentHeadElement[] content) =>
			new Document(docContent.Concat(content.SelectMany(x => x.Build())));

		public IDocumentWithHead Body(params IDocumentBodyElement[] content) =>
			new Document(docContent.Concat(content.SelectMany(x => x.Build())));

		public IEnumerable<string> Build() => new Content(docContent).Build();
	}

	internal class Content : ILines
	{
		private readonly IEnumerable<string> content;

		public Content(IEnumerable<string> content)
		{
			this.content = content;
		}

		public IEnumerable<string> Build() => content;

		public IEnumerator<string> GetEnumerator() => content.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => content.GetEnumerator();
	}

}
