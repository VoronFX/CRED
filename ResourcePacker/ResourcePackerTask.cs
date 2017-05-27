using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace ResourcePacker
{
	public class ResourcePackerTask : Task
	{
		[Required]
		public string IntermediateAssembly { get; set; }

		public override bool Execute()
		{
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
			Log.LogError("aaaaaas2adad");
			return false;
		}
	}
}
