using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using dotless.Core.configuration;
using Microsoft.Build.Framework;

namespace CRED.BuildTasks
{
	public sealed class LessCompiler : TaskBase
	{
		[Required]
		[ExpandPath]
		[NonNullArray]
		[DataMember]
		public string[] InputFiles { get; set; }

		//[Required]
		//[NormalizeDirectoryPath]
		//[DataMember]
		//public string RootDirectory { get; set; }

		[Required]
		[NormalizeDirectoryPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputDirectory { get; set; }

		[DataMember]
		public bool Debug { get; set; }

		protected override bool ExecuteWork()
		{
			BuildIncrementally(InputFiles, inputFiles =>
			{
				var config = new DotlessConfiguration
				{
					Debug = Debug,
				};

				return InputFiles
					.AsParallel()
					.AsOrdered()
					.Select(file =>
					{
						var outFile = Path.GetFullPath(Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(file) + ".css"));
						File.WriteAllText(outFile, dotless.Core.Less.Parse(File.ReadAllText(file), config));
						return outFile;

					}).ToArray();

			});

			return true;
		}

	}
}