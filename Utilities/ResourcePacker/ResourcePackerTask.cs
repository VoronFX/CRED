using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

		public override bool Execute()
		{
			var projectDir = Path.GetFullPath(ProjectDir);
			var outputDir = Path.Combine(projectDir, GenerationOutputDir);
			if (Directory.Exists(outputDir))
				Directory.Delete(outputDir, true);

			foreach (var fileGroup in InputFiles
				//.Where(x => !Path.IsPathRooted(x))
				.Select(x => Path.Combine(projectDir, x))
				.Where(x => x.StartsWith(projectDir) && !x.StartsWith(outputDir))
				.GroupBy(x => Path.GetDirectoryName(x.Substring(ProjectDir.Length + 1))))
			{
				var directories = fileGroup.Key.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				var path = Path.Combine(outputDir,
					string.Join(Path.DirectorySeparatorChar.ToString(), directories.Take(directories.Length - 1)),
					directories.Last().ToPascalCaseIdentifier() + ".cs");

				Directory.CreateDirectory(Path.GetDirectoryName(path));
				//File.WriteAllText(path,
				//	new MapFileTemplate("Namespace", directories, fileGroup
				//			.Select(x => new MapFileTemplate.Item(Path.GetFileName(x).ToPascalCaseIdentifier(), x, new[] { x })))
				//		.TransformText());


				var classes = directories.Select(x =>
					new CodeTypeDeclaration(x.ToPascalCaseIdentifier())
					{
						
						TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract,
						//StartDirectives = { new CodeDirective(){ "static" } }
						IsPartial = true,
						IsClass = true,
					}
				).ToArray();

				CodeCommentStatement SummaryComment(params string[] commentLines) =>
					new CodeCommentStatement(
						new XDocument(
							new XElement("summary",
								new object[] { Environment.NewLine }
									.Concat(commentLines.Select(x => new XText(x)))
									.Concat(new[] { Environment.NewLine })
									.ToArray())
						).ToString(), true);

				var pathConst = new CodeMemberField(typeof(string), "Path")
				{
					// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
					Attributes = MemberAttributes.Const | MemberAttributes.Public,
					Comments = { SummaryComment("Path to this directory", fileGroup.Key) },
					InitExpression = new CodePrimitiveExpression(fileGroup.Key)
				};

				var fileConsts = fileGroup
					.Select(x => x.Substring(projectDir.Length + 1 + fileGroup.Key.Length))
					.Select(x => new CodeMemberField(typeof(string), Path.GetFileName(x).ToPascalCaseIdentifier())
					{
						// ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
						Attributes = MemberAttributes.Const | MemberAttributes.Public,
						Comments = { SummaryComment(x) },
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
					GetStatements = { new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(string),
						fileConsts.Select(x => new CodeArgumentReferenceExpression(x.Name))
							.Cast<CodeExpression>()
							.ToArray())) }
				};

				classes.Last().Members.Add(pathConst);
				classes.Last().Members.AddRange(fileConsts);
				classes.Last().Members.Add(allFilesArray);

				var unit = new CodeCompileUnit()
				{
					Namespaces =
					{
						new CodeNamespace("Resources")
						{
							Types =
							{
								classes.Reverse().Aggregate((seed, parent) =>
								{
									parent.Members.Add(seed);
									return parent;
								})
							}
						}
					}
				};

				using (var writer = new StringWriter())
				using (var provider = CodeDomProvider.CreateProvider("CSharp"))
				{
					provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions
					{
						//Keep the braces on the line following the statement or 
						//declaration that they are associated with);
						BracingStyle = "C"
					});

					File.WriteAllText(path, writer.ToString());
				}
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
	}
}
