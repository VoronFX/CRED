

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CRED.BuildTasks.IncrementalBuild;
using CsCodeGenerator;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CRED.BuildTasks
{
	[DataContract]
	public abstract class TaskBase : Task
	{
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string IncrementalBuildCacheFile { get; set; }

		[DataMember]
		[NormalizeDirectoryPath]
		public string RelativeRoot { get; set; }

		[DataMember]
		public bool DebugBreak { get; set; }

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class NonNullArrayAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class EnsureDirectoryCreatedAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class ExpandPathAttribute : Attribute
		{
		}

		[AttributeUsage(AttributeTargets.Property)]
		public sealed class NormalizeDirectoryPathAttribute : Attribute
		{
		}

		private static IEnumerable<PropertyInfo> PropertiesWithAttribute(Type targetType, Type attributeType)
		{
			return targetType.GetTypeInfo().GetRuntimeProperties()
				.Except(typeof(TaskBase).BaseType.GetRuntimeProperties())
				.Where(x => x.CustomAttributes
					.Any(a => a.AttributeType == attributeType));
		}

		private void ProcessPathProperties(Type attributeSelector, Func<string, string> process)
		{
			foreach (var property in PropertiesWithAttribute(GetType(), attributeSelector))
			{
				if (property.PropertyType == typeof(string))
				{
					var path = (string)property.GetValue(this);
					if (!string.IsNullOrWhiteSpace(path))
						property.SetValue(this, process(path));
				}
				else if (property.PropertyType == typeof(string[]))
				{
					var array = (string[])property.GetValue(this);
					for (var i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrWhiteSpace(array[i]))
							array[i] = process(array[i]);
					}
				}
			}
		}

		public sealed override bool Execute()
		{
			try
			{
				if (DebugBreak)
				{
					LogError("DebugBreak before processing properties");
				}

				foreach (var property in PropertiesWithAttribute(GetType(), typeof(NonNullArrayAttribute)))
				{
					if (property.PropertyType.IsArray
						&& property.GetValue(this) == null)
					{
						property.SetValue(this, Array.CreateInstance(property.PropertyType.GetElementType(), 0));
					}
				}

				ProcessPathProperties(typeof(ExpandPathAttribute),
					path => Path.GetFullPath(Path.Combine(RelativeRoot, path)));

				ProcessPathProperties(typeof(NormalizeDirectoryPathAttribute),
					path => Path.GetFullPath(path + Path.DirectorySeparatorChar));

				ProcessPathProperties(typeof(EnsureDirectoryCreatedAttribute),
					path =>
					{
						IOExtension.EnsureFileDirectoryCreated(path);
						return path;
					});

				if (DebugBreak)
				{
					LogError("DebugBreak after processing properties");
					return false;
				}
				return ExecuteWork();
			}
			catch (Exception e)
			{
				LogError(e.ToString());
				return false;
			}
		}

		private void LogError(string message)
		{
			Log.LogError(string.Join(Environment.NewLine,
				message,
				$"{GetType().Name}:",
				JsonConvert.SerializeObject(this, Formatting.Indented)
			));
		}

		protected abstract bool ExecuteWork();

		public bool BuildIncrementally(ICollection<string> inputFiles,
			Func<ICollection<string>, IEnumerable<string>> build)
		{
			if (!Cache.CheckNeedBuild(IncrementalBuildCacheFile, inputFiles, this, out Cache cache))
			{
				Log.LogMessage(MessageImportance.High, $"Skipping {GetType().Name} task: {IncrementalBuildCacheFile}");
				return false;
			}
			Log.LogMessage(MessageImportance.High, $"------ Running {GetType().Name} task: {IncrementalBuildCacheFile} ------");

			var outputFiles = build(inputFiles).ToArray();

			if (!string.IsNullOrWhiteSpace(IncrementalBuildCacheFile))
				cache.SaveBuildCache(IncrementalBuildCacheFile, outputFiles);

			cache.OutputFiles
				.AsParallel()
				.Select(x => x.Path)
				.Except(outputFiles.AsParallel())
				.Where(File.Exists)
				.ForAll(File.Delete);

			return true;
		}

	}
}