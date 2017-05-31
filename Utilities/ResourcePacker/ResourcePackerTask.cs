using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ResourcePacker
{
	public class ResourcePackerTask : Task
	{
		[Required]
		public string IntermediateAssembly { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string[] InputFiles { get; set; }

		public string GenerationOutputDir { get; set; }

		public enum OutputMode
		{
			Structured,
			Flat,
			OneFile
		}

		public OutputMode Mode { get; set; }

		public override bool Execute()
		{
			var projectDir = Path.GetFullPath(ProjectDir);
			var outputDir = Path.Combine(projectDir, GenerationOutputDir);
			if (Directory.Exists(outputDir))
				Directory.Delete(outputDir, true);

			var classesHeirarchy = new Dictionary<string[], CodeTypeDeclaration[]>(StringArrayValueEqualityComparer.Instance);

			CodeTypeDeclaration NewClass(string name)
				=> new CodeTypeDeclaration(name)
				{
					TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract,
					IsPartial = true,
					IsClass = true,
				};

			CodeCompileUnit NewUnit(CodeTypeDeclaration contentMember)
				=> new CodeCompileUnit()
				{
					Namespaces =
					{
						new CodeNamespace("Resources")
						{
							Types = { contentMember }
						}
					}
				};			

			var filesInDirectories = InputFiles
				.Select(x => Path.Combine(projectDir, x))
				.Where(x => x.StartsWith(projectDir) && !x.StartsWith(outputDir))
				.GroupBy(x => Path.GetDirectoryName(x.Substring(ProjectDir.Length + 1)),
					(directory, files) =>
				{
					var pathDirectories = directory.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					var pathIdentifiers = pathDirectories.Select(d => d.ToPascalCaseIdentifier()).ToArray();

					CodeTypeDeclaration[] GetOrCreateClasses(string[] names, bool sharedHeirarchy)
					{
						if (!sharedHeirarchy || !classesHeirarchy.TryGetValue(names, out CodeTypeDeclaration[] classes))
						{
							var newClass = NewClass(names.Last());

							if (names.Length > 1)
							{
								var parents =
									GetOrCreateClasses(names.Take(names.Length - 1).ToArray(), sharedHeirarchy);
								parents.Last().Members.Add(newClass);
								classes = parents.Concat(new[] { newClass }).ToArray();
							}
							else
							{
								classes = new[] { newClass };
							}

							if (sharedHeirarchy)
								classesHeirarchy.Add(names, classes);
						}
						return classes;
					}

					var pathClasses = GetOrCreateClasses(new []{ "ResourceMap" }.Concat(pathIdentifiers).ToArray(), Mode == OutputMode.OneFile);
					FillResourceClass(directory, files, projectDir, pathClasses.Last());
					
					return new
					{
						PathDirectories = pathDirectories,
						PathIdentifiers = pathDirectories.Select(d => d.ToPascalCaseIdentifier()).ToArray(),
						Classes = pathClasses
					};
				}).ToArray();

			switch (Mode)
			{
				case OutputMode.Structured:
				case OutputMode.Flat:

					foreach (var file in filesInDirectories)
					{
						var path = Path.Combine(outputDir, string.Join(
							Mode == OutputMode.Flat ? "." : Path.DirectorySeparatorChar.ToString(),
							file.PathIdentifiers.Take(file.PathDirectories.Length - 1)),
							file.PathIdentifiers.Last().ToPascalCaseIdentifier() + ".ResMap.cs");

						Directory.CreateDirectory(Path.GetDirectoryName(path));

						WriteCode(NewUnit(file.Classes.First()), path);
					}

					break;
				case OutputMode.OneFile:

					var oneFilePath = Path.Combine(outputDir,"ResourceMap.cs");
					Directory.CreateDirectory(Path.GetDirectoryName(oneFilePath));

					WriteCode(NewUnit(filesInDirectories.First().Classes.First()), oneFilePath);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			//var module = ModuleDefinition.ReadModule(IntermediateAssembly);
			//foreach (var attribute in module.Types
			//	.Where(x => x.HasCustomAttributes)
			//	.SelectMany(x => x.CustomAttributes)
			//	.Where(x => x.AttributeType.FullName == typeof(RequireResourceAttribute).FullName
			//	            && x.HasConstructorArguments)
			//	.Select(x => x.ConstructorArguments.First())
			//	.Where(x => x.Type.FullName == typeof(string).FullName))
			//{
			//	Console.WriteLine(attribute.Value);
			//	//module.Resources.Add(new EmbeddedResource("", ManifestResourceAttributes.Public, ));
			//	//attribute.ConstructorArguments[0].
			//	//foreach (CustomAttribute attribute in type.CustomAttributes)
			//	//{
			//	//	if (attribute.AttributeType.FullName != attributeType)
			//	//		continue;

			//	//	result = attribute;
			//	//	return true;
			//	//}

			//	//return false;

			//	//Console.WriteLine(type.FullName);
			//}




			//Log.LogError("Hello, wsorld!");
			//File.Delete(IntermediateAssembly);
			//var assembly = Assembly.ReflectionOnlyLoadFrom(IntermediateAssembly);
			return false;
		}

		private static void WriteCode(CodeCompileUnit unit, string path)
		{
			using (var writer = new StringWriter())
			using (var provider = CodeDomProvider.CreateProvider("CSharp"))
			{
				provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions
				{
					//Keep the braces on the line following the statement or 
					//declaration that they are associated with);
					BracingStyle = "C"
				});

				var code = writer.ToString();

				code = Regex.Replace(code, @"(?m)(?<=^[\sa-z]*?)sealed\s+?abstract(?=[\sa-z]+?class)", "static");

				File.WriteAllText(path, code);
			}
		}

		private class StringArrayValueEqualityComparer : IEqualityComparer<string[]>
		{
			public static StringArrayValueEqualityComparer Instance { get; }
				= new StringArrayValueEqualityComparer();

			public bool Equals(string[] a, string[] b)
			{
				if (a == null && b == null)
					return true;
				if (a == null || b == null)
					return false;
				return a.Length == b.Length &&
					   a.Select((aValue, index) => aValue == b[index])
						   .All(x => x);
			}

			public int GetHashCode(string[] obj) => obj.GetHashCode();
		}

		private static CodeCommentStatement SummaryComment(params string[] commentLines)
			=> new CodeCommentStatement(
				new XDocument(
					new XElement("summary",
						new object[] { Environment.NewLine }
							.Concat(commentLines.Select(x => new XText(x)))
							.Concat(new[] { Environment.NewLine })
							.ToArray())).ToString(), true);

		private static void FillResourceClass(string directoryPath, IEnumerable<string> files, string projectDir, CodeTypeDeclaration targetClass)
		{
			var pathConst = new CodeMemberField(typeof(string), "Path")
			{
				// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Const | MemberAttributes.Public,
				Comments = { SummaryComment("Path to this directory ", directoryPath) },
				InitExpression = new CodePrimitiveExpression(directoryPath)
			};

			var fileConsts = files
				.Select(x => x.Substring(projectDir.Length + 1 + directoryPath.Length))
				.Select(x => new CodeMemberField(typeof(string), Path.GetFileName(x).ToPascalCaseIdentifier())
				{
					// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
					Attributes = MemberAttributes.Const | MemberAttributes.Public,
					Comments = { SummaryComment(directoryPath + x) },
					InitExpression = new CodeBinaryOperatorExpression(
						new CodeArgumentReferenceExpression(pathConst.Name),
						CodeBinaryOperatorType.Add,
						new CodePrimitiveExpression(x))
				})
				.Cast<CodeTypeMember>()
				.ToArray();

			var allFilesArray = new CodeMemberProperty()
			{
				Type = new CodeTypeReference(typeof(string[])),
				Name = "All",
				// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Static | MemberAttributes.Public,
				Comments = { SummaryComment("Collection of all files in this directory") },
				GetStatements =
				{
					new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(string),
						fileConsts.Select(x => new CodeArgumentReferenceExpression(x.Name))
							.Cast<CodeExpression>()
							.ToArray()))
				}
			};

			targetClass.Members.Add(pathConst);
			targetClass.Members.AddRange(fileConsts);
			targetClass.Members.Add(allFilesArray);
		}
	}
}
