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
		public Cache(FileStamp[] inputFiles, FileStamp[] outputFiles, JObject parameters, Guid assemblyModuleVersionId)
		{
			InputFiles = inputFiles;
			OutputFiles = outputFiles;
			Parameters = parameters;
			AssemblyModuleVersionId = assemblyModuleVersionId;
		}

		public FileStamp[] InputFiles { get; set; }
		public FileStamp[] OutputFiles { get; set; }
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

		public static bool CheckNeedBuild(string cacheFile, IEnumerable<string> inputFiles, object parameters,
			out Cache cache)
		{
			var taskParameters = new Lazy<JObject>(() => JObject.FromObject(parameters), LazyThreadSafetyMode.None);

			var input = inputFiles.ToArray();

			var assemblyModuleVersionId = typeof(Cache).GetTypeInfo().Assembly.ManifestModule.ModuleVersionId;

			var cacheLoadSuccess = TryLoad(cacheFile, out cache);

			bool needBuild = !cacheLoadSuccess;

			var inputPathsEqual = !cacheLoadSuccess || input
				                      .AsParallel()
				                      .AsOrdered()
				                      .SequenceEqual(cache.InputFiles
					                      .AsParallel()
					                      .AsOrdered()
					                      .Select(x => x.Path));

			needBuild = needBuild || !inputPathsEqual;

			needBuild = needBuild || cache.AssemblyModuleVersionId != assemblyModuleVersionId;

			needBuild = needBuild || cache.Parameters.Equals(taskParameters.Value);

			FileStamp[] newInputFilesStamps;

			if (inputPathsEqual)
			{
				newInputFilesStamps =
					cache.InputFiles
						.AsParallel()
						.AsOrdered()
						.Select(x =>
						{
							var newStamp = x.NewStampIfRealFileChanged();
							if (newStamp != null)
								Volatile.Write(ref needBuild, true);
							return newStamp ?? x;
						})
						.ToArray();
			}
			else
			{
				newInputFilesStamps = input
					.AsParallel()
					.AsOrdered()
					.Select(x => new FileStamp(x))
					.ToArray();
			}

			if (needBuild)
			{
				cache = new Cache(newInputFilesStamps, new FileStamp[] { },
					taskParameters.Value, assemblyModuleVersionId);

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
				.Select(x => new FileStamp(x))
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