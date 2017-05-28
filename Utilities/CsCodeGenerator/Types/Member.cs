using System;
using System.Collections.Generic;
using System.Linq;
using CsCodeGenerator.Interfaces.Common;
using MemberInt = CsCodeGenerator.Interfaces.Member;
using NamespaceInt = CsCodeGenerator.Interfaces.Namespace;

namespace CsCodeGenerator.Types
{
	internal enum MemberType
	{
		Class,
		Struct,
		Interface,
		Enum,
		Property,
		Field,
		Method,
		Constant
	}

	internal partial class Member :
		NamespaceInt.IClass,
		NamespaceInt.IStruct,
		NamespaceInt.IInterface,
		NamespaceInt.IEnum,
		MemberInt.IClass,
		MemberInt.IStruct,
		MemberInt.IInterface,
		MemberInt.IEnum,
		MemberInt.IProperty,
		MemberInt.IField,
		MemberInt.IMethod,
		MemberInt.IConstant
	{
		public IEnumerable<IElement> Content { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Parameters { get; set; }
		public bool ExpressionBody { get; set; }
		public Modifiers Modifiers { get; set; }
		public bool IsSealed => Modifiers.HasFlag(Modifiers.Sealed);
		public bool IsStatic => Modifiers.HasFlag(Modifiers.Static);
		public bool IsReadonly => Modifiers.HasFlag(Modifiers.Reaonly);
		public bool IsPartial => Modifiers.HasFlag(Modifiers.Partial);
		public MemberType MemberType { get; set; }

		private string BuildAccessModifiers()
		{
			switch (Modifiers | 
				(Modifiers)System.Enum.GetValues(typeof(AccessModifiers)).Cast<AccessModifiers>()
						.Aggregate(default(AccessModifiers), (seed, value) => seed | value))
			{
				case Modifiers.Public:
					return "public ";
				case Modifiers.Protected:
					return "protected ";
				case Modifiers.Internal:
					return "internal ";
				case Modifiers.Private:
					return "private ";
				case Modifiers.ProtectedInternal:
					return "protected internal ";
				default:
					return string.Empty;
			}
		}

		private string BuildMemberName()
		{
			switch (MemberType)
			{
				case MemberType.Class:
					return "class " + Name;
				case MemberType.Struct:
					return "struct " + Name;
				case MemberType.Interface:
					return "interface " + Name;
				case MemberType.Enum:
					return "enum " + Name;
				case MemberType.Property:
					return Type + " " + Name +
						(ExpressionBody ? " => " :
						(IsReadonly ? " { get; } = " : " { get; set; } = "));
				case MemberType.Field:
					return Type + " " + Name + " = ";
				case MemberType.Method:
					return Type + " " + Name + "(" + Parameters + ")";
				case MemberType.Constant:
					return Type + " " + "const " + Name;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public IEnumerable<string> Build()
		{
			var head = BuildAccessModifiers()
					   + (IsSealed ? "sealed " : null)
					   + (IsStatic ? "static " : null)
					   + (IsReadonly && MemberType == MemberType.Field ? "readonly " : null)
					   + (IsPartial ? "partial " : null)
					   + (MemberType == MemberType.Constant ? "const " : null)
					   + BuildMemberName();

			switch (MemberType)
			{
				case MemberType.Class:
				case MemberType.Struct:
				case MemberType.Interface:
				case MemberType.Enum:
				case MemberType.Method:
					return new[] { head, "{" }.Concat(
							Content.SelectMany(x => x.Build()).Indent())
						.Concat(new[] { "}" });
				case MemberType.Property:
				case MemberType.Field:
				case MemberType.Constant:
					var content = Content.SelectMany(x => x.Build()).ToArray();
					return content.First().Length + head.Length > 100 ?
						new[] { head }.Concat(content.Indent()) :
						new[] { head + content.First() }.Concat(content.Skip(1).Indent()); ;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		//private Member SetNameAndContent(string name, MemberInt.IMember content) =>
		//	SetNameAndContent(name, new[] { content });

		//public Member SetMemberTypeNameAndContent(MemberType type, string name, IEnumerable<MemberInt.IMember> content) =>
		//	this.Set(_ => _.Name = name)
		//		.Set(_ => _.MemberType = type)
		//		.Set(_ => _.Content = content);
		
		//public Member SetModifiers(Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers) => 
		//	this.Set(_ => _.Modifiers = modifiers(new ModifiersBuilder()).Modifiers);

		//public Member SetModifiers(Func<IModifiersBuilder, INestedMemberModifiers> modifiers) =>
		//	this.Set(_ => _.Modifiers = modifiers(new ModifiersBuilder()).Modifiers);

		//private Member SetMember(string type, string name, IEnumerable<string> content) =>
		//	this.Set(_ => _.Name = name)
		//		.Set(_ => _.Type = type)
		//		.Set(_ => _.Content = new[] { new Content(content) });

		//private Member SetMember(string type, string name, string content) =>
		//	SetMember(type, name, new[] { content });

		//public Member Class(string name, Func<IModifiersBuilder, ITopLevelMemberModifiers> modifiers, MemberInt.IMember[] content) =>
		//	this.Set(_ => _.MemberType = MemberType.Class)
		//		.SetNameAndContent(name, content);

		//public Member Class(string name, Func<IModifiersBuilder, INestedMemberModifiers> modifiers, MemberInt.IMember[] content) =>
		//	this.Set(_ => _.MemberType = MemberType.Class)
		//		.SetNameAndContent(name, content);

		//public Member Struct(string name, MemberInt.IMember content) => Struct(name, new[] { content });

		//public Member Struct(string name, IEnumerable<MemberInt.IMember> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Struct)
		//		.SetNameAndContent(name, content);

		//public Member Interface(string name, MemberInt.IMember content) => Interface(name, new[] { content });

		//public Member Interface(string name, IEnumerable<MemberInt.IMember> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Interface)
		//		.SetNameAndContent(name, content);

		//public Member Enum(string name, MemberInt.IMember content) => Enum(name, new[] { content });

		//public Member Enum(string name, IEnumerable<MemberInt.IMember> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Enum)
		//		.SetNameAndContent(name, content);

		//public Member Field(string type, string name, string content) =>
		//	Field(type, name, new[] { content });

		//public Member Field(string type, string name, IEnumerable<string> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Field)
		//		.SetMember(type, name, content);

		//public Member Property(string type, string name, bool expressionBody, string content) =>
		//	Property(type, name, expressionBody, new[] { content });

		//public Member Property(string type, string name, bool expressionBody, IEnumerable<string> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Property)
		//		.Set(_ => _.ExpressionBody = expressionBody)
		//		.SetMember(type, name, content);

		//public Member Method(string returnType, string parameters, string name, bool expressionBody, string content) =>
		//	Method(returnType, parameters, name, expressionBody, new[] { content });

		//public Member Method(string returnType, string parameters, string name, bool expressionBody, IEnumerable<string> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Method)
		//		.Set(_ => _.ExpressionBody = expressionBody)
		//		.Set(_ => _.Parameters = parameters)
		//		.SetMember(returnType, name, content);

		//public Member Const(string type, string name, string content) =>
		//	Const(type, name, new[] { content });

		//public Member Const(string type, string name, IEnumerable<string> content) =>
		//	this.Set(_ => _.MemberType = MemberType.Constant)
		//		.SetMember(type, name, content);

	}
}