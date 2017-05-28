using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace ResourcePacker
{
	public class ResourcePackerTaskWrapper : Task
	{
		public string TaskAssemblyRelativePath = @"\bin\Debug\net46\";
		public string TaskAssemblyFileName = "ResourcePacker.dll";
		public string TaskAssemblyDebugSymbolsFileName = "ResourcePacker.pdb";

		[Required]
		public string MSBuildThisFileDirectory { get; set; }

		[Required]
		public string IntermediateAssembly { get; set; }

		[Required]
		public string ProjectDir { get; set; }

		[Required]
		public string[] InputFiles { get; set; }

		[Required]
		public string GenerationOutputDir { get; set; }

		public override bool Execute()
		{
			foreach (var inputFile in InputFiles)
			{
			Log.LogError(inputFile);

			}
			var path = MSBuildThisFileDirectory + TaskAssemblyRelativePath;

			var assembly = File.Exists(path + TaskAssemblyDebugSymbolsFileName) ?
				Assembly.Load(
					File.ReadAllBytes(path + TaskAssemblyFileName),
					File.ReadAllBytes(path + TaskAssemblyDebugSymbolsFileName)) :
				Assembly.Load(File.ReadAllBytes(path + TaskAssemblyFileName));

			var type = assembly.GetType("ResourcePacker.ResourcePackerTask");

			Log.LogError("AAAAAAAAAAA ($SolutionDir) ss" + MSBuildThisFileDirectory);

			//var instance = Activator.CreateInstance(Type);
			//Log.LogError(Type.GetMethod(nameof(Task.Execute)).Invoke(instance, null).ToString());
			dynamic instance = Activator.CreateInstance(type);
			//     into it.
			instance.BuildEngine = BuildEngine;
			instance.HostObject = HostObject;

			instance.IntermediateAssembly = IntermediateAssembly;
			instance.ProjectDir = ProjectDir;
			instance.InputFiles = InputFiles;
			instance.GenerationOutputDir = GenerationOutputDir;

			//instance.WriteError = new Action<string>(s => BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", "", 0, 0, 0, 0, string.Format("Pepita: {0}", s), "", "Pepita")));
			//instance.SolutionDirectory = @"$(SolutionDir)";
			//instance.WriteInfo = new Action<string>(s => BuildEngine.LogMessageEvent(new BuildMessageEventArgs(s, "", "Pepita", MessageImportance.High)));
			//instance.ProjectDirectory = @"$(ProjectDir)";
			return instance.Execute();
			//return (bool)Type.GetMethod(nameof(Task.Execute)).Invoke(instance, null);
			//   } 
			//    catch (Exception e) 
			//   {
			//     Log.LogError(e.Message); 
			//    }     
		}

	}
}
