using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace CRED.BuildTasks.IncrementalBuild
{
	public struct Cache
	{
		[JsonConstructor]
		public Cache(FileTimestamp[] inputFiles, FileTimestamp[] outputFiles, JObject parameters, Guid assemblyModuleVersionId)
		{
			InputFiles = inputFiles;
			OutputFiles = outputFiles;
			Parameters = parameters;
			AssemblyModuleVersionId = assemblyModuleVersionId;
		}

		public FileTimestamp[] InputFiles { get; set; }
		public FileTimestamp[] OutputFiles { get; set; }
		public JObject Parameters { get; set; }
		public Guid AssemblyModuleVersionId { get; set; }

		private static bool TryLoad(string path, out Cache cache)
		{
			cache = default(Cache);

			if (!File.Exists(path))
				return false;

			try
			{
				//var jobject = JObject.Parse(File.ReadAllText(path));

				//if (jobject.IsValid(new JSchemaGenerator().Generate(typeof(Cache))))
				//{
				//	cache = jobject.ToObject<Cache>();
				//	return true;
				//}
				cache = JsonConvert.DeserializeObject<Cache>(File.ReadAllText(path));
				return true;
			}
			catch (JsonReaderException)
			{

			}
			return false;
		}

		private void Save(string path)
		{
			File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		public static bool CheckNeedBuild(string cacheFile, IEnumerable<string> inputFiles, object parameters, out Cache cache)
		{
			var taskParameters = new Lazy<JObject>(() => JObject.FromObject(parameters), LazyThreadSafetyMode.None);

			var input = new Lazy<Lazy<FileTimestamp>[]>(() =>
					inputFiles
						.AsParallel()
						.Where(path => !string.IsNullOrWhiteSpace(path))
						.Distinct()
						.Where(File.Exists)
						.OrderBy(path => path)
						.Select(path => new Lazy<FileTimestamp>(() => FileTimestamp.FromFile(path), LazyThreadSafetyMode.None))
						.ToArray(),
				LazyThreadSafetyMode.None);

			var assemblyModuleVersionId = typeof(Cache).GetTypeInfo().Assembly.ManifestModule.ModuleVersionId;

			bool needBuild = !TryLoad(cacheFile, out cache);

			needBuild = needBuild || cache.AssemblyModuleVersionId != assemblyModuleVersionId;

			needBuild = needBuild || cache.Parameters.Equals(taskParameters.Value);

			needBuild = needBuild || !input.Value
				            .AsParallel()
				            .AsOrdered()
				            .Select(x => x.Value)
				            .SequenceEqual(cache.InputFiles.AsParallel().AsOrdered());

			needBuild = needBuild || cache.InputFiles
				            .Concat(cache.OutputFiles)
				            .AsParallel()
				            .Any(x => x.CheckRealFileChanged());

			if (needBuild)
			{
				cache = new Cache(input.Value
						.AsParallel()
						.AsOrdered()
						.Select(x => x.Value)
						.ToArray(),
					new FileTimestamp[] { }, taskParameters.Value, assemblyModuleVersionId);

				return true;
			}

			return false;
		}

		public void SaveBuildCache(string cacheFile, IEnumerable<string> outputFiles)
		{
			var output = outputFiles
				.AsParallel()
				.Where(path => !string.IsNullOrWhiteSpace(path))
				.Distinct()
				.Where(File.Exists)
				.OrderBy(path => path)
				.Select(FileTimestamp.FromFile)
				.ToArray();

			OutputFiles
				.AsParallel()
				.Select(x => x.Path)
				.Except(output.AsParallel().Select(x => x.Path))
				.Where(File.Exists)
				.ForAll(File.Delete);

			new Cache(InputFiles, output, Parameters, AssemblyModuleVersionId)
				.Save(cacheFile);
		}
	}
}