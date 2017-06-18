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
	public sealed class CssClassesMapper : TaskBase
	{
		[Required]
		[ExpandPathAttribute]
		[DataMember]
		public string[] InputFiles { get; set; }

		[Required]
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputFile { get; set; }

		[DataMember]
		public string Namespace { get; set; }

		[DataMember]
		public string ClassName { get; set; }

		protected override bool ExecuteWork()
		{
			if (string.IsNullOrWhiteSpace(ClassName))
				ClassName = "CssClassesMap";
			InputFiles = InputFiles ?? Array.Empty<string>();

			BuildIncrementally(InputFiles, inputFiles =>
			{
				var classes = InputFiles
						.AsParallel()
						.AsOrdered()
						.SelectMany(file =>
							Regex.Matches(
									Regex.Replace(File.ReadAllText(file), @"(?is)\{.*?\}", " "),
									@"(?is)\.[A-Z_a-z0-9-]+")
								.Cast<Match>()
								.Select(x => new { Class = x.Value.Substring(1), File = file }))
						.GroupBy(x => x.Class, x => x.File, (key, files) =>
							new KeyValuePair<string, IEnumerable<string>>(key, files.Distinct()));

				File.WriteAllLines(OutputFile, GenerateCssClassesMap(Namespace, ClassName, classes));
				return new[] { OutputFile };
			});

			return true;
		}

		public static IEnumerable<string> GenerateCssClassesMap(string @namespace, string className, IEnumerable<KeyValuePair<string, IEnumerable<string>>> classes)
		{
			var classesConsts = classes
				.SelectMany(cssClass => new[]
					{
						string.Empty,
						"/// <summary>",
						"/// "+ cssClass.Key.XmlEscape(),
						"/// Referenced in next css files:"
					}
					.Concat(cssClass.Value.Select(x =>
						"/// " + x.XmlEscape()
					))
					.Concat(new[]{

						"/// </summary>",
						$"public const string {cssClass.Key.ToPascalCaseIdentifier()} = {cssClass.Key.ToVerbatimLiteral()};"
					}));

			var mapClass = new[]
				{
					string.Empty,
					$"public static class {className}",
					"{",
				}
				.Concat(classesConsts.Indent())
				.Concat(new[]
				{
					"}"
				});

			return Generator.Flatten(
				Generator.GeneratedHeader,
				new[]
				{
					string.Empty,
					$"namespace {@namespace}",
					"{",
				},
				mapClass.Indent(),
				new[]
				{
					"}"
				});
		}


	}
}