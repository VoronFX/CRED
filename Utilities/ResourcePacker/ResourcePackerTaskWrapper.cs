using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace ResourcePacker
{
	public class ResourcePackerTaskWrapper : DynamicObject, ITask
	{
		private Lazy<object> TaskInstance { get; }

		public ResourcePackerTaskWrapper()
		{
			TaskInstance = new Lazy<object>(() =>
			{
				var path = TaskAssemblyPath;

				var assembly = File.Exists(path + TaskAssemblyDebugSymbolsFileName)
					? Assembly.Load(
						File.ReadAllBytes(path + TaskAssemblyFileName),
						File.ReadAllBytes(path + TaskAssemblyDebugSymbolsFileName))
					: Assembly.Load(File.ReadAllBytes(path + TaskAssemblyFileName));

				var type = assembly.GetType("ResourcePacker.ResourcePackerTask");

				return Activator.CreateInstance(type);
			}, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public string TaskAssemblyRelativePath = @"\bin\Debug\net46\";
		public string TaskAssemblyFileName = "ResourcePacker.dll";
		public string TaskAssemblyDebugSymbolsFileName = "ResourcePacker.pdb";

		public string TaskAssemblyPath { get; set; }

		//[Required]
		//public string MSBuildThisFileDirectory { get; set; }

		//[Required]
		//public string IntermediateAssembly { get; set; }

		//[Required]
		//public string ProjectDir { get; set; }

		//[Required]
		//public string[] InputFiles { get; set; }

		//[Required]
		//public string GenerationOutputDir { get; set; }

		//public override bool Execute()
		//{
		//	foreach (var inputFile in InputFiles)
		//	{
		//		Log.LogError(inputFile);

		//	}
		//	var path = MSBuildThisFileDirectory + TaskAssemblyRelativePath;

		//	var assembly = File.Exists(path + TaskAssemblyDebugSymbolsFileName) ?
		//		Assembly.Load(
		//			File.ReadAllBytes(path + TaskAssemblyFileName),
		//			File.ReadAllBytes(path + TaskAssemblyDebugSymbolsFileName)) :
		//		Assembly.Load(File.ReadAllBytes(path + TaskAssemblyFileName));

		//	var type = assembly.GetType("ResourcePacker.ResourcePackerTask");

		//	Log.LogError("AAAAAAAAAAA ($SolutionDir) ss" + MSBuildThisFileDirectory);

		//	//var instance = Activator.CreateInstance(Type);
		//	//Log.LogError(Type.GetMethod(nameof(Task.Execute)).Invoke(instance, null).ToString());
		//	dynamic instance = Activator.CreateInstance(type);

		//	//     into it.
		//	instance.BuildEngine = BuildEngine;
		//	instance.HostObject = HostObject;

		//	instance.IntermediateAssembly = IntermediateAssembly;
		//	instance.ProjectDir = ProjectDir;
		//	instance.InputFiles = InputFiles;
		//	instance.GenerationOutputDir = GenerationOutputDir;

		//	//instance.WriteError = new Action<string>(s => BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", "", 0, 0, 0, 0, string.Format("Pepita: {0}", s), "", "Pepita")));
		//	//instance.SolutionDirectory = @"$(SolutionDir)";
		//	//instance.WriteInfo = new Action<string>(s => BuildEngine.LogMessageEvent(new BuildMessageEventArgs(s, "", "Pepita", MessageImportance.High)));
		//	//instance.ProjectDirectory = @"$(ProjectDir)";
		//	return instance.Execute();
		//	//return (bool)Type.GetMethod(nameof(Task.Execute)).Invoke(instance, null);
		//	//   } 
		//	//    catch (Exception e) 
		//	//   {
		//	//     Log.LogError(e.Message); 
		//	//    }     
		//}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var method = TaskInstance.Value.GetType().GetRuntimeMethod(binder.Name, args.Select(x => x.GetType()).ToArray());
			if (method != null)
			{
				result = method.Invoke(TaskInstance.Value, args);
				return true;
			}
			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			var property = TaskInstance.Value.GetType().GetRuntimeProperty(binder.Name);
			if (property != null)
			{
				property.SetValue(TaskInstance.Value, value);
				return true;
			}
			var field = TaskInstance.Value.GetType().GetRuntimeField(binder.Name);
			if (field != null)
			{
				field.SetValue(TaskInstance.Value, value);
				return true;
			}
			return false;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var property = TaskInstance.Value.GetType().GetRuntimeProperty(binder.Name);
			if (property != null)
			{
				result = property.GetValue(TaskInstance.Value);
				return true;
			}
			var field = TaskInstance.Value.GetType().GetRuntimeField(binder.Name);
			if (field != null)
			{
				result = field.GetValue(TaskInstance.Value);
				return true;
			}
			result = null;
			return false;
		}

		public bool Execute() => ((dynamic)TaskInstance.Value).Execute();

		public IBuildEngine BuildEngine
		{
			get => ((dynamic)TaskInstance.Value).BuildEngine;
			set => ((dynamic)TaskInstance.Value).BuildEngine = value;
		}

		public ITaskHost HostObject
		{
			get => ((dynamic)TaskInstance.Value).HostObject;
			set => ((dynamic)TaskInstance.Value).HostObject = value;
		}
	}
}
