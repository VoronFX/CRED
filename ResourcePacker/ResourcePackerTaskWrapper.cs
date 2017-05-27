using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace ResourcePacker
{
	public class ResourcePackerTaskWrapper : Task
	{
		[Required]
		public string MSBuildThisFileDirectory { get; set; }

		public override bool Execute()
		{
			var path = MSBuildThisFileDirectory + @"\bin\Debug\";
			var assembly = File.Exists(path + "ResourcePacker.pdb") ? Assembly.Load(
				  File.ReadAllBytes(path + "ResourcePacker.dll"),
				  File.ReadAllBytes(path + "ResourcePacker.pdb")):
				Assembly.Load(File.ReadAllBytes(path + @"ResourcePacker.dll"));

			var type = assembly.GetType("ResourcePacker.ResourcePackerTask");

			Log.LogError("s2adad" + MSBuildThisFileDirectory);

			//var instance = Activator.CreateInstance(Type);
			//Log.LogError(Type.GetMethod(nameof(Task.Execute)).Invoke(instance, null).ToString());
			dynamic instance = Activator.CreateInstance(type);
			//     into it.
			instance.BuildEngine = BuildEngine;
			instance.HostObject = HostObject;
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
