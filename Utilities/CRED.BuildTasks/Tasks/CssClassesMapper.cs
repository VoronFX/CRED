using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsCodeGenerator;

namespace CRED.BuildTasks.Tasks
{
	public sealed class CssClassesMapperTask : TaskRunner.Task
	{
		private CssClassesMapper Task { get; }

		public CssClassesMapperTask(CssClassesMapper classesMapper)
		{
			Task = classesMapper;
			if (string.IsNullOrWhiteSpace(Task.ClassName))
				Task.ClassName = "CssClassesMap";
			Task.InputFiles = Task.InputFiles ?? Array.Empty<string>();
		}

		public override void Execute()
		{
			Task.BuildIncrementally(Task.InputFiles, inputFiles =>
			{
				var classes = Task.InputFiles
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

				File.WriteAllLines(Task.OutputFile, GenerateCssClassesMap(Task.Namespace, Task.ClassName, classes));
				return new[] { Task.OutputFile };
			});
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
					"// ReSharper disable InconsistentNaming",
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