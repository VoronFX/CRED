using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CsCodeGenerator
{
	public static class Generator
	{
		public static IEnumerable<string> Namespace(string name, string[] comment, IEnumerable<string> imports, IEnumerable<string> content)
			=> imports.Select(i => $"using {i};")
				.Concat(new[] { String.Empty })
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
						@public ? nameof(@public) : String.Empty,
						@sealed ? nameof(@sealed) : String.Empty,
						@static ? nameof(@static) : String.Empty,
						partial ? nameof(partial) : String.Empty, "class", name),
					"{"
				})
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}",
					String.Empty
				});

		public static IEnumerable<string> Property(string name, string value, string[] comment,
			bool @const, bool @static, bool field, bool @public = true, bool @readonly = true, string type = "string")
			=> Comment(comment)
				.Concat(new[]
				{
					JoinNonEmpty(" ",
						@public ? nameof(@public) : String.Empty,
						@static ? nameof(@static) : String.Empty,
						@const ? nameof(@const) : String.Empty,
						@readonly && field ? nameof(@readonly) : String.Empty,
						type, name,
						field ? "=" : (@readonly? "{ get; } =" :"{ get; set; } ="), value+";"),
					String.Empty
				});

		[Flags]
		public enum AccessModifiers
		{
			Public,
			Private,
			Internal,
			Protected,
		}

		public enum MemberType
		{
			Class,
			Struct,
			Interface,
			Enum,
			Method,
			Field,
			Property
		}

		public class Member
		{

			public AccessModifiers AccessModifiers { get; set; }
			public MemberType Type { get; set; }

			public bool Sealed { get; set; }
			public bool Static { get; set; }
			public bool Abstract { get; set; }
			public bool Virtual { get; set; }
			public bool New { get; set; }
			public bool Override { get; set; }
			public bool Readonly { get; set; }

			public bool Assignment { get; set; }
			public bool SameLine { get; set; }
			public bool ExpressionBody { get; set; }

			public string Name { get; set; }
			public string ReturnType { get; set; }
			public IEnumerable<string> Comment { get; set; }
			public IEnumerable<string> InheritedTypes { get; set; }
			public IEnumerable<string> Parameters { get; set; }
			public IEnumerable<string> GenericParameters { get; set; }
			public IEnumerable<string> Content { get; set; }

			public IEnumerable<string> Build()
			{
				var result = new List<string>();
				result.AddRange(Comment);
				result.Add(JoinNonEmpty(" ",
					AccessModifiers.HasFlag(AccessModifiers.Public) ? "public" : String.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Private) ? "private" : String.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Internal) ? "internal" : String.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Protected) ? "protected" : String.Empty,

					Static || (Abstract && Sealed) ? "static" : String.Empty,
					Sealed && !(Abstract && Sealed) ? "sealed" : String.Empty,
					Abstract && !(Abstract && Sealed) ? "abstract" : String.Empty,
					Virtual ? "virtual" : String.Empty,
					Readonly ? "readonly" : String.Empty,
					New ? "new" : String.Empty,
					Override ? "override" : String.Empty,

					Type == MemberType.Class ? "class" : String.Empty,
					Type == MemberType.Struct ? "struct" : String.Empty,
					Type == MemberType.Interface ? "interface" : String.Empty,
					Type == MemberType.Enum ? "enum" : String.Empty,
					ReturnType,
					Name + (GenericParameters != null ? $"<{String.Join(", ", GenericParameters)}>" : String.Empty),
					Type == MemberType.Class ? $"({String.Join(", ", Parameters)})" : String.Empty,
					InheritedTypes != null ? $": {String.Join(", ", InheritedTypes)}" : String.Empty,
					ExpressionBody ? "=>" : String.Empty,
					Assignment ? "=" : String.Empty));

				var brackets = !ExpressionBody && (Type == MemberType.Class || Type == MemberType.Struct ||
												   Type == MemberType.Interface || Type == MemberType.Enum ||
												   Type == MemberType.Method);
				if (brackets)
					result.Add("{");


				if (SameLine && !brackets)
				{
				var content = Content.ToArray();
					result[result.Count - 1] += content.FirstOrDefault();
					result.AddRange(content.Skip(1));
				}
				else
				{
					result.AddRange(Content);
				}
				if (brackets)
					result.Add("}");
				return result;
			}

			public Member()
			{ }
			public Member(TypeInfo prototype)
			{
				Name = prototype.Name;
				Type = MemberType.Struct;
				Type = prototype.IsClass ? MemberType.Class : Type;
				Type = prototype.IsInterface ? MemberType.Class : Type;
				Type = prototype.IsEnum ? MemberType.Class : Type;
				AccessModifiers |= (prototype.IsPublic || prototype.IsNestedPublic) ? AccessModifiers.Public : 0;
				AccessModifiers |= prototype.IsNestedPrivate ? AccessModifiers.Private : 0;
				if (prototype.BaseType != null)
					InheritedTypes = new[] { prototype.BaseType.Name };
				InheritedTypes = InheritedTypes.Concat(prototype.ImplementedInterfaces.Select(x => x.Name));
				Abstract = prototype.IsAbstract;
				Sealed = prototype.IsSealed;
			}

			public Member(MethodInfo prototype)
			{
				Name = prototype.Name;
				Type = MemberType.Method;
				AccessModifiers |= (prototype.IsPublic || prototype.IsPrivate) ? AccessModifiers.Public : 0;
				ReturnType = prototype.ReturnType.ToString();
				Parameters = prototype.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}");
				Abstract = prototype.IsAbstract;
				Virtual = prototype.IsVirtual;
			}

		}

		public static IEnumerable<string> Type<T>(IEnumerable<string> content)
			=> Type<T>(typeof(T).Name, content);

		public static IEnumerable<string> Type<T>(string name, IEnumerable<string> content)
			=> Type(name, typeof(T).GetTypeInfo(), content);

		private static IEnumerable<string> Type(string name, TypeInfo typeInfo, IEnumerable<string> content)
			=> new[]
				{
					JoinNonEmpty(" ",
						typeInfo.IsPublic ? "public" : "internal",
						typeInfo.IsSealed && !typeInfo.IsAbstract ? "sealed" : String.Empty,
						typeInfo.IsSealed && typeInfo.IsAbstract ? "static": String.Empty,
						typeInfo.IsAbstract && !typeInfo.IsSealed ? "abstract": String.Empty,
						typeInfo.IsClass ? "class" : String.Empty,
						typeInfo.IsInterface ? "interface" : String.Empty,
						typeInfo.IsEnum ? "enum" : String.Empty,
						!typeInfo.IsClass && !typeInfo.IsInterface && !typeInfo.IsEnum ? "struct" : String.Empty),
					"{"
				}
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}",
					String.Empty
				});

		public static IEnumerable<string> Comment(string[] comment) =>
			comment.Any() ?
				new[] { "/// <summary>" }

					.Concat(comment.Select(c => "/// " + new XElement("dummy", c).Value))
					.Concat(new[] { "/// </summary>" }) : new string[] { };

		public static IEnumerable<string> Indent(this IEnumerable<string> input)
			=> input.Select(x => "    " + x);

		private static string JoinNonEmpty(string separator, params string[] strings)
			=> String.Join(separator, strings.Where(s => !String.IsNullOrWhiteSpace(s)));

		public static string Flatten(this IEnumerable<string> strings)
			=> String.Join(Environment.NewLine, strings);

		public static IEnumerable<string> Flatten(params IEnumerable<string>[] lines) 
			=> lines.SelectMany(x => x);

		public static string ToPascalCaseIdentifier(this string name)
		{
			var result = Regex.Replace("-" + name, "(?si)[^A-Za-z0-9]+", "-");
			result = Regex.Replace(result, "(?si)-+([A-Za-z0-9]?)",
				x => x.Groups[1].Value.ToUpperInvariant());
			if (!char.IsLetter(result[0]))
				result = "_" + result;
			return result;
		}

		public static string ToVerbatimLiteral(this string input)
			=> $@"@""{input.Replace("\"", "\"\"")}""";

		public static IEnumerable<string> UnindentVerbatim(this string input)
		{
			var splitted = input.Replace("\r\n", "\n").Split('\n');
			var indent = splitted
				.Where(x => !String.IsNullOrWhiteSpace(x))
				.Select(x => x.Length - x.TrimStart().Length)
				.Min();

			return splitted.Select(x => String.IsNullOrWhiteSpace(x) ? x : x.Substring(indent));
		}

		public static string XmlEscape(this string input)
			=> new XElement("dummy", input).Value;

		//public static string ToLiteral(this string input)
		//{
		//	using (var writer = new StringWriter())
		//	using (var provider = CodeDomProvider.CreateProvider("CSharp"))
		//	{
		//		provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
		//		//provider.GenerateCodeFromMember(
		//		// new CodeMemberField()
		//		// {
		//		//  Attributes = MemberAttributes.Const,
		//		//  Type = new CodeTypeReference(typeof(string)),
		//		//  Comments = { new CodeCommentStatement("sdasdas", true) }
		//		// }, writer, null);
		//		return writer.ToString();
		//	}
		//}

		public static IEnumerable<string> GeneratedHeader { get; } = @"
				//------------------------------------------------------------------------------
				// <auto-generated>
				//     This code was generated by a tool.
				//
				//     Changes to this file may cause incorrect behavior and will be lost if
				//     the code is regenerated.
				// </auto-generated>
				//------------------------------------------------------------------------------
			".UnindentVerbatim();
	}
}
