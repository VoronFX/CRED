using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using CsCodeGenerator;
using Microsoft.Build.Framework;
using Newtonsoft.Json;

namespace CRED.BuildTasks
{
	public sealed class ManifestGenerator : TaskBase
	{
		[Required]
		[NormalizeDirectoryPath]
		[DataMember]
		public string PathSubtractDirectory { get; set; }

		[Required]
		[ExpandPath]
		[NonNullArray]
		[DataMember]
		public string[] InputFiles { get; set; }

		[Required]
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputFile { get; set; }

		[DataMember]
		public string Namespace { get; set; }

		protected override bool ExecuteWork()
		{
			BuildIncrementally(InputFiles, inputFiles =>
			{
				FileUtilities.ThrowIfOutsideOfDirectoryTree(PathSubtractDirectory, InputFiles);

				File.WriteAllText(OutputFile, JsonConvert.SerializeObject(new
				{
					Resources =
					InputFiles
						.AsParallel()
						.AsOrdered()
						.Select(file => 
							file.Substring(PathSubtractDirectory.Length)
								.Replace(Path.DirectorySeparatorChar, '/')
								+"?v="+FileUtilities.GetHashForFile(file))
						.ToArray()
				}, Formatting.Indented));

				return new[] { OutputFile };
			});

			return true;
		}

	}

}