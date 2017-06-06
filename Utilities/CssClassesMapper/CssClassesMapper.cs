using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsCodeGenerator;
using static CsCodeGenerator.Generator;

namespace CssClassesMapper
{
	public partial class CssClassesMapper
	{
		private CssClassesMapperTask Task { get; }

		static CssClassesMapper()
		{
			Execute = (task) => new CssClassesMapper(task).MapClassses();
		}

		private CssClassesMapper(CssClassesMapperTask task)
		{
			Task = task;

			Task.InputFiles = Task.InputFiles ?? Array.Empty<string>();
		}

		private void MapClassses()
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

			IOExtension.EnsureFileDirectoryCreated(Task.OutputFile);
			File.WriteAllLines(Task.OutputFile, GenerateCssClassesMap(Task.Namespace, Task.ClassName, classes));
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

			return Flatten(
				GeneratedHeader,
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