using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
					AccessModifiers.HasFlag(AccessModifiers.Public) ? "public" : string.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Private) ? "private" : string.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Internal) ? "internal" : string.Empty,
					AccessModifiers.HasFlag(AccessModifiers.Protected) ? "protected" : string.Empty,

					Static || (Abstract && Sealed) ? "static" : string.Empty,
					Sealed && !(Abstract && Sealed) ? "sealed" : string.Empty,
					Abstract && !(Abstract && Sealed) ? "abstract" : string.Empty,
					Virtual ? "virtual" : string.Empty,
					Readonly ? "readonly" : string.Empty,
					New ? "new" : string.Empty,
					Override ? "override" : string.Empty,

					Type == MemberType.Class ? "class" : string.Empty,
					Type == MemberType.Struct ? "struct" : string.Empty,
					Type == MemberType.Interface ? "interface" : string.Empty,
					Type == MemberType.Enum ? "enum" : string.Empty,
					ReturnType,
					Name + (GenericParameters != null ? $"<{string.Join(", ", GenericParameters)}>" : string.Empty),
					Type == MemberType.Class ? $"({string.Join(", ", Parameters)})" : string.Empty,
					InheritedTypes != null ? $": {string.Join(", ", InheritedTypes)}" : string.Empty,
					ExpressionBody ? "=>" : string.Empty,
					Assignment ? "=" : string.Empty));

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
						typeInfo.IsSealed && !typeInfo.IsAbstract ? "sealed" : string.Empty,
						typeInfo.IsSealed && typeInfo.IsAbstract ? "static": string.Empty,
						typeInfo.IsAbstract && !typeInfo.IsSealed ? "abstract": string.Empty,
						typeInfo.IsClass ? "class" : string.Empty,
						typeInfo.IsInterface ? "interface" : string.Empty,
						typeInfo.IsEnum ? "enum" : string.Empty,
						!typeInfo.IsClass && !typeInfo.IsInterface && !typeInfo.IsEnum ? "struct" : string.Empty),
					"{"
				}
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}",
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
