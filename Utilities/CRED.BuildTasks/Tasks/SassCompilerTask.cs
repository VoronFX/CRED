using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using LibSass.Compiler.Options;
using Microsoft.Build.Framework;

namespace CRED.BuildTasks
{
	public sealed class SassCompiler : TaskBase
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
				return EntryFiles
					.Select(file =>
					{
						var outFile = Path.GetFullPath(Path.Combine(OutputDirectory, Path.GetFileNameWithoutExtension(file) + ".css"));

						var sassCompiler = new LibSass.Compiler.SassCompiler(new SassOptions()
						{
							IncludeSourceComments = Debug,
							EmbedSourceMap = Debug,
							InputPath = file
						});

						var result = sassCompiler.Compile();

						//var options = new CompilationOptions
						//{
						//	SourceMap = Debug,
						//	SourceComments = Debug,
						//	InlineSourceMap = Debug,
						//};

						//var result = LibSassHost.SassCompiler.Compile(File.ReadAllText(file), options);

						//var result = Scss.ConvertFileToCss(file, new ScssOptions
						//{
						//	InputFile = file,
						//	OutputFile = outFile,
						//	GenerateSourceMap = Debug,
						//	SourceComments = Debug,
						//	SourceMapEmbed = Debug
						//});
						
						File.WriteAllText(outFile, result.Output);
						return outFile;

					}).ToArray();
			});

			return true;
		}

	}
}