using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using CRED.BuildTasks.Wrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace CRED.BuildTasks
{
	public static class IncrementalBuild
	{
		private sealed class Cache
		{
			public Cache(FileTimestamp[] inputFiles, FileTimestamp[] outputFiles, string parameters, Guid assemblyModuleVersionId)
			{
				InputFiles = inputFiles;
				OutputFiles = outputFiles;
				Parameters = parameters;
				AssemblyModuleVersionId = assemblyModuleVersionId;
			}

			public FileTimestamp[] InputFiles { get; }
			public FileTimestamp[] OutputFiles { get; }
			public string Parameters { get; }
			public Guid AssemblyModuleVersionId { get; }

			public static Cache Load(string path)
			{
				if (!File.Exists(path))
					return null;

				try
				{
					var jobject = JObject.Parse(File.ReadAllText(path));
					return jobject.IsValid(new JSchemaGenerator().Generate(typeof(Cache)))
						? jobject.ToObject<Cache>()
						: null;
				}
				catch (JsonReaderException)
				{
					return null;
				}
			}

			public void Save(string path)
			{
				File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
			}
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
		private sealed class FileTimestamp
		{
			private bool Equals(FileTimestamp other)
			{
				return string.Equals(Path, other.Path) && Timestamp.Equals(other.Timestamp);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				return obj is FileTimestamp && Equals((FileTimestamp)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ Timestamp.GetHashCode();
				}
			}

			public static FileTimestamp FromFile(string path) 
				=> new FileTimestamp(System.IO.Path.GetFullPath(path), File.GetLastWriteTimeUtc(path));

			public bool CheckRealFileChanged()
				=> !File.Exists(Path) || File.GetLastWriteTimeUtc(Path) != Timestamp;

			public FileTimestamp(string path, DateTime timestamp)
			{
				Path = path;
				Timestamp = timestamp;
			}

			public string Path { get; }
			public DateTime Timestamp { get; }
		}

		public static bool BuildIncrementally(this TaskWrapperBase task, ICollection<string> inputFiles,
			Func<ICollection<string>, IEnumerable<string>> build)
		{
			var cache = Cache.Load(task.IncrementalBuildCacheFile);

			var taskParameters = new Lazy<string>(task.Serialize, LazyThreadSafetyMode.None);

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

			bool needBuild = cache == null;

			needBuild = needBuild || cache.AssemblyModuleVersionId != assemblyModuleVersionId;

			needBuild = needBuild || cache.Parameters != taskParameters.Value;

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
				var taskParametersCopy = taskParameters.Value;

				var outputFiles = build(inputFiles)
					.AsParallel()
					.Where(path => !string.IsNullOrWhiteSpace(path))
					.Distinct()
					.Where(File.Exists)
					.OrderBy(path => path)
					.Select(FileTimestamp.FromFile)
					.ToArray();

				cache?.OutputFiles
					.AsParallel()
					.Select(x => x.Path)
					.Except(outputFiles.AsParallel().Select(x => x.Path))
					.Where(File.Exists)
					.ForAll(File.Delete);

				if (!string.IsNullOrWhiteSpace(task.IncrementalBuildCacheFile))
				{
					new Cache(input.Value
								.AsParallel()
								.AsOrdered()
								.Select(x => x.Value)
								.ToArray(),
							outputFiles, taskParametersCopy, assemblyModuleVersionId)
						.Save(task.IncrementalBuildCacheFile);
				}
			}

			return needBuild;
		}
	}
}