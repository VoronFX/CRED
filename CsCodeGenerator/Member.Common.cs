using System;
using System.Collections.Generic;
using System.Linq;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Member;

namespace CsCodeGenerator.Types
{
	internal partial class Member : IElement
	{
		private IEnumerable<IElement> Content { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Parameters { get; set; }
		public bool ExpressionBody { get; set; }
		public AccessModifiers AccessModifiers { get; set; }
		public bool IsSealed { get; set; }
		public bool IsStatic { get; set; }
		public bool IsReadonly { get; set; }
		public bool IsPartial { get; set; }
		public MemberType MemberType { get; set; }

		private string BuildAccessModifiers()
		{
			switch (AccessModifiers)
			{
				case AccessModifiers.Public:
					return "public ";
				case AccessModifiers.Protected:
					return "protected ";
				case AccessModifiers.Internal:
					return "internal ";
				case AccessModifiers.Private:
					return "private ";
				case AccessModifiers.ProtectedInternal:
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
				case MemberType.Struc:
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
				case MemberType.Struc:
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

		public Member Public() => this.Set(_ => _.AccessModifiers = AccessModifiers.Public);
		public Member Internal() => this.Set(_ => _.AccessModifiers = AccessModifiers.Internal);
		public Member Protected() => this.Set(_ => _.AccessModifiers = AccessModifiers.Protected);
		public Member Private() => this.Set(_ => _.AccessModifiers = AccessModifiers.Private);
		public Member ProtectedInternal() => this.Set(_ => _.AccessModifiers = AccessModifiers.ProtectedInternal);

		public Member Sealed() => this.Set(_ => _.IsSealed = true);
		public Member Readonly() => this.Set(_ => _.IsReadonly = true);
		public Member Static() => this.Set(_ => _.IsStatic = true);
		public Member Partial() => this.Set(_ => _.IsPartial = true);

		private Member SetNameAndContent(string name, IMember content) =>
			SetNameAndContent(name, new[] { content });

		private Member SetNameAndContent(string name, IEnumerable<IMember> content) =>
			this.Set(_ => _.Name = name)
				.Set(_ => _.Content = content);


		private Member SetMember(string type, string name, IEnumerable<string> content) =>
			this.Set(_ => _.Name = name)
				.Set(_ => _.Type = type)
				.Set(_ => _.Content = new[] { new Content(content) });

		private Member SetMember(string type, string name, string content) =>
			SetMember(type, name, new[] { content });

		public Member Class(string name, IMember content) => Class(name, new[] { content });

		public Member Class(string name, IEnumerable<IMember> content) =>
			this.Set(_ => _.MemberType = MemberType.Class)
				.SetNameAndContent(name, content);

		public Member Struct(string name, IMember content) => Struct(name, new[] { content });

		public Member Struct(string name, IEnumerable<IMember> content) =>
			this.Set(_ => _.MemberType = MemberType.Struc)
				.SetNameAndContent(name, content);

		public Member Interface(string name, IMember content) => Interface(name, new[] { content });

		public Member Interface(string name, IEnumerable<IMember> content) =>
			this.Set(_ => _.MemberType = MemberType.Interface)
				.SetNameAndContent(name, content);

		public Member Enum(string name, IMember content) => Enum(name, new[] { content });

		public Member Enum(string name, IEnumerable<IMember> content) =>
			this.Set(_ => _.MemberType = MemberType.Enum)
				.SetNameAndContent(name, content);

		public Member Field(string type, string name, string content) =>
			Field(type, name, new[] { content });

		public Member Field(string type, string name, IEnumerable<string> content) =>
			this.Set(_ => _.MemberType = MemberType.Field)
				.SetMember(type, name, content);

		public Member Property(string type, string name, bool expressionBody, string content) =>
			Property(type, name, expressionBody, new[] { content });

		public Member Property(string type, string name, bool expressionBody, IEnumerable<string> content) =>
			this.Set(_ => _.MemberType = MemberType.Property)
				.Set(_ => _.ExpressionBody = expressionBody)
				.SetMember(type, name, content);

		public Member Method(string returnType, string parameters, string name, bool expressionBody, string content) =>
			Method(returnType, parameters, name, expressionBody, new[] { content });

		public Member Method(string returnType, string parameters, string name, bool expressionBody, IEnumerable<string> content) =>
			this.Set(_ => _.MemberType = MemberType.Method)
				.Set(_ => _.ExpressionBody = expressionBody)
				.Set(_ => _.Parameters = parameters)
				.SetMember(returnType, name, content);

		public Member Const(string type, string name, string content) =>
			Const(type, name, new[] { content });

		public Member Const(string type, string name, IEnumerable<string> content) =>
			this.Set(_ => _.MemberType = MemberType.Constant)
				.SetMember(type, name, content);

	}
}