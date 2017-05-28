using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Document;
using Namespace = CsCodeGenerator.Interfaces.Namespace;
using Member = CsCodeGenerator.Interfaces.Member;
using CsCodeGenerator.Types;

namespace CsCodeGenerator
{
	public static class Fluent
	{
		private static Modifiers GetModifiers(Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers) =>
			modifiers(new ModifiersBuilder()).Modifiers;

		private static Modifiers GetModifiers(Func<IModifiersBuilder, INestedMemberModifiers> modifiers) =>
			modifiers(new ModifiersBuilder()).Modifiers;

		private static Types.Member NewMember(MemberType type, string name, Modifiers modifiers, IEnumerable<IElement> content) =>
			new Types.Member()
			.Set(_ => _.MemberType = type)
			.Set(_ => _.Name = name)
			.Set(_ => _.Modifiers = modifiers)
			.Set(_ => _.Content = content);

		public static Namespace.IClass Class(string name, Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Class, name, GetModifiers(modifiers), content);

		public static Member.IClass Class(string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Class, name, GetModifiers(modifiers), content);

		public static Namespace.IStruct Struct(string name, Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Struct, name, GetModifiers(modifiers), content);

		public static Member.IStruct Struct(string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Struct, name, GetModifiers(modifiers), content);

		public static Namespace.IEnum Enum(string name, Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Enum, name, GetModifiers(modifiers), content);

		public static Member.IEnum Enum(string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Enum, name, GetModifiers(modifiers), content);

		public static Namespace.IInterface Interface(string name, Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Interface, name, GetModifiers(modifiers), content);

		public static Member.IInterface Interface(string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			Member.IMember[] content) => NewMember(MemberType.Interface, name, GetModifiers(modifiers), content);

		public static Member.IProperty Property(string type, string name, bool expressionBody, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			string[] content) => NewMember(MemberType.Property, name, GetModifiers(modifiers), new[] { new Content(content) })
			.Set(_ => _.ExpressionBody = expressionBody)
			.Set(_ => _.Type = type);

		public static Member.IMethod Method(string returnType, string parameters, string name, bool expressionBody, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			string[] content) => NewMember(MemberType.Method, name, GetModifiers(modifiers), new[] { new Content(content) })
			.Set(_ => _.ExpressionBody = expressionBody)
			.Set(_ => _.Parameters = parameters)
			.Set(_ => _.Type = returnType);

		public static Member.IField Field(string type, string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			string[] content) => NewMember(MemberType.Field, name, GetModifiers(modifiers), new[] { new Content(content) })
			.Set(_ => _.Type = type);

		public static Member.IConstant Constant(string type, string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers,
			string[] content) => NewMember(MemberType.Constant, name, GetModifiers(modifiers), new[] { new Content(content) })
			.Set(_ => _.Type = type);

		public static IDocument Document() => new Document();
		public static INamespace Namespace(string name, params Namespace.INamespaceElement[] elements) => new Types.Namespace(name, elements);
		public static IUsing Using(string @namespace) => new Using(false, @namespace);
		public static IUsing UsingStatic(string type) => new Using(true, type);
		public static IUsing UsingAlias(string alias, string target) => new Using(alias, target);
		public static IComment SummaryComment(params string[] content) => new SummaryComment(content);
		public static IComment Comment(params string[] content) => new Comment(content);
		public static ILines Lines(params string[] content) => new Content(content);
		public static ILines EmptyLine(params string[] content) => new Content(new[] { string.Empty });
	}
}
