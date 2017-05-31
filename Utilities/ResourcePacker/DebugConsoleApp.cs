using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ResourcePacker
{
	internal class DebugConsoleApp
	{
		private static void Main(string[] args)
		{
			//var s = new MapFileTemplate("Dummy", new[] {"Fxs", "Portal"}, new MapFileTemplate.Item[]
			//{
			//	new MapFileTemplate.Item("Kiss", "sdasda\\aaaa\"", new []{"sadalkjsdal", "sada"}),
			//}).TransformText();

			new ResourcePackerTask()
			{				
				InputFiles = Directory.GetFiles(@"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure\extracted\", "*", SearchOption.AllDirectories)
					.Select(x => x.Substring(@"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure\".Length))
					.ToArray(),
				IntermediateAssembly = "",
				GenerationOutputDir = @"sources",
				ProjectDir = @"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure",
				Mode = ResourcePackerTask.OutputMode.OneFile
			}.Execute();
			return;
			Console.ReadKey();

			//new ResourcePackerTaskWrapper()
			//{
			//	TaskAssemblyRelativePath = @"\",
			//	TaskAssemblyFileName = Assembly.GetExecutingAssembly().GetName().Name + ".exe",
			//	TaskAssemblyDebugSymbolsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".pdb",
			//	MSBuildThisFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
			//	InputFiles = Directory.GetFiles(@"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure\extracted\", "*", SearchOption.AllDirectories)
			//		.Select(x => x.Substring(@"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure\".Length))
			//		.ToArray(),
			//	IntermediateAssembly = "",
			//	GenerationOutputDir = @"sources",
			//	ProjectDir = @"C:\Users\Voron\Source\Repos\CRED\CRED\wwwroot\azure"
			//}.Execute();
			Console.ReadKey();
		}
	}
}
