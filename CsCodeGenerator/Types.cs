using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces;

namespace CsCodeGenerator
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

	internal class SummaryComment : Comment
	{
		public SummaryComment(IEnumerable<string> content)
			: base(new[] { "<summary>" }
				.Concat(content.Select(x => new XElement("dummy", x).Value))
				.Concat(new[] { "</summary>" }))
		{
		}
	}

	internal class Comment : Content, IComment
	{
		public Comment(IEnumerable<string> content)
			: base(content.Select(x => "/// " + x))
		{
		}
	}

	internal class Content : IElement
	{
		private readonly string[] content;

		protected Content(IEnumerable<string> content)
		{
			this.content = content.ToArray();
		}

		public IEnumerable<string> Build() => content;
	}

	internal class Member :
		IElement,
		IPublic,
		IInternal,
		IProtected,
		IPrivate,
		IProtectedInternal
	{
		private List<IElement> Elements { get; }
			= new List<IElement>();

		public AccessModifiers AccessModifiers { get; set; }
		public bool Sealed { get; set; }
		public bool Static { get; set; }
		public bool Readonly { get; set; }
		public MemberType MemberType { get; set; }

	}
}
