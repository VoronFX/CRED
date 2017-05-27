using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Namespace;
using CsCodeGenerator.Types;

namespace CsCodeGenerator
{
	public static class Fluent
	{
		public static IPublic Public() => new Member().Public();
		public static IInternal Internal() => new Member().Internal();
		public static IProtected Protected() => new Member().Protected();
		public static IPrivate Private() => new Member().Private();
		public static IProtectedInternal ProtectedInternal() => new Member().ProtectedInternal();

		public static IDocument Document(
			IEnumerable<IDocumentHeadElement> headContent, IDocumentBodyElement bodyContent)
			=> new Document(headContent, new[] { bodyContent });

		public static IDocument Document(
			IEnumerable<IDocumentHeadElement> headContent,
			IEnumerable<IDocumentBodyElement> bodyContent) => new Document(headContent, bodyContent);

		public static INamespace Namespace(string name, INamespaceElement element) => Namespace(name, new[] { element });
		public static INamespace Namespace(string name, IEnumerable<INamespaceElement> elements) => new Namespace(name, elements);
		public static IUsing Using(IEnumerable<string> content) => new Using(content);
		public static IUsing Using(string content) => new Using(new[] { content });
		public static IComment SummaryComment(IEnumerable<string> content) => new SummaryComment(content);
		public static IComment SummaryComment(string content) => new SummaryComment(new[] { content });
		public static IComment Comment(IEnumerable<string> content) => new Comment(content);
		public static IComment Comment(string content) => new Comment(new[] { content });
		public static IEmptyLine EmptyLine(string content) => new EmptyLine();
	}
}
