using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

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
				.GroupBy(x => Path.GetDirectoryName(x.Substring(ProjectDir.Length+1))))
			{
				var directories = fileGroup.Key.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				var path = Path.Combine(outputDir, 
					string.Join(Path.DirectorySeparatorChar.ToString(), directories.Take(directories.Length - 1)),
					directories.Last().ToPascalCaseIdentifier() + ".cs");

				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllText(path,
					new MapFileTemplate("Namespace", directories.Select(x => x), fileGroup
							.Select(x => new MapFileTemplate.Item(Path.GetFileName(x).ToPascalCaseIdentifier(), x, new[] { x })))
						.TransformText());
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
