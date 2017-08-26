using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using dotless.Core;
using dotless.Core.configuration;
using dotless.Core.Loggers;
using dotless.Core.Parser;
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

		[Required]
		[ExpandPath]
		[NonNullArray]
		[DataMember]
		public string[] EntryFiles { get; set; }

		[Required]
		[ExpandPath]
		[NormalizeDirectoryPath]
		[DataMember]
		public string RootPath { get; set; }

		[Required]
		[NormalizeDirectoryPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputDirectory { get; set; }

		[DataMember]
		public bool Debug { get; set; }

		private Lazy<LessEngine> LessEngine { get; }

		public LessCompiler()
		{
			LessEngine = new Lazy<LessEngine>(() =>
				new LessEngine(new Parser
				{
					CurrentDirectory = RootPath,
					Debug = Debug
				}), LazyThreadSafetyMode.PublicationOnly);
		}

		protected override bool ExecuteWork()
		{
			BuildIncrementally(InputFiles, inputFiles =>
			{
				return EntryFiles
					.Select(file =>
					{
						var outFile = Path.GetFullPath(Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(file) + ".css"));
						var css = LessEngine.Value.TransformToCss(File.ReadAllText(file), file);
						if (!LessEngine.Value.LastTransformationSuccessful)
						{
							throw LessEngine.Value.LastTransformationError;
						}
						File.WriteAllText(outFile, css);
						return outFile;

					}).ToArray();
			});

			return true;
		}

	}
}