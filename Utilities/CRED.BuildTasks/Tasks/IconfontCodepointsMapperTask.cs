using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using CsCodeGenerator;
using Microsoft.Build.Framework;

namespace CRED.BuildTasks
{
	public sealed class IconfontCodepointsMapper : ValueMapper
	{
		protected override bool ExecuteWork()
		{
			if (string.IsNullOrWhiteSpace(ClassName))
				ClassName = "IconfontCodepointMap";

			BuildIncrementally(InputFiles, inputFiles =>
			{
				var valueItems = InputFiles
					.AsParallel()
					.AsOrdered()
					.SelectMany(file => File.ReadAllLines(file)
								.Where(line => !string.IsNullOrWhiteSpace(line))
								.Select(line => line.Split(' ').First())
					)
					.Select(x =>
						new ValueMapItem(x, x, new string[] { })
					);

				File.WriteAllLines(OutputFile, GenerateValueMap(Namespace, ClassName, valueItems));
				return new[] { OutputFile };
			});

			return true;
		}


	}
}