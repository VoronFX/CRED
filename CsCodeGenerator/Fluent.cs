using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces;

namespace CsCodeGenerator
{
	public static class Fluent
	{
		private static T Set<T>(T obj, Action<T> action)
		{
			action(obj);
			return obj;
		}

		public static IPublic Public() => Set(new Member(), _ => _.AccessModifiers = AccessModifiers.Public);
		public static IInternal Internal() => Set(new Member(), _ => _.AccessModifiers = AccessModifiers.Internal);
		public static IProtected Protected() => Set(new Member(), _ => _.AccessModifiers = AccessModifiers.Protected);
		public static IPrivate Private() => Set(new Member(), _ => _.AccessModifiers = AccessModifiers.Private);
		public static IProtectedInternal ProtectedInternal() => Set(new Member(), _ => _.AccessModifiers = AccessModifiers.ProtectedInternal);

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
