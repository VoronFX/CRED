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
	public sealed class CssClassesMapper : ValueMapper
	{
		protected override bool ExecuteWork()
		{
			if (string.IsNullOrWhiteSpace(ClassName))
				ClassName = "CssClassesMap";

			BuildIncrementally(InputFiles, inputFiles =>
			{
			var valueItems = InputFiles
				.AsParallel()
				.AsOrdered()
				.SelectMany(file =>
					{
						var css = File.ReadAllText(file);
						css = Regex.Replace(css, @"(?is)\{.*?\}", " ");
						css = Regex.Replace(css, @"(?i)//.*?", " ");
						css = Regex.Replace(css, @"(?is)/\*.*?\*/", " ");
						return Regex.Matches(css, @"(?is)\.[A-Z_a-z0-9-]+")
							.Cast<Match>()
							.Select(x => new { Class = x.Value.Substring(1), File = file });
					}
				)
				.GroupBy(x => x.Class, x => x.File, FromClasses);

			File.WriteAllLines(OutputFile, GenerateValueMap(Namespace, ClassName, valueItems));
			return new[] { OutputFile };
		});

			return true;
		}

	public static ValueMapItem FromClasses(string cssClassName, IEnumerable<string> files) =>
		new ValueMapItem(cssClassName, cssClassName, new[] { cssClassName, "Referenced in next css files:" }
			.Concat(files.Distinct())
			.ToArray());


}
}