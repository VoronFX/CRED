using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using CRED.BuildTasks.Wrapper;
using CsCodeGenerator;
using Microsoft.AspNetCore.WebUtilities;
using ResourceMapper.Base;

namespace CRED.BuildTasks.Tasks.ResourceMapper
{
	public sealed class ResourceMapperTask : TaskRunner.Task
	{
		private BuildTasks.ResourceMapper Task { get; }

		public ResourceMapperTask(BuildTasks.ResourceMapper task)
		{
			Task = task;
			if (string.IsNullOrWhiteSpace(Task.Namespace))
				Task.Namespace = nameof(ResourceMapper);
			if (string.IsNullOrWhiteSpace(Task.TopClassName))
				Task.TopClassName = "ResourceMap";
			Task.InputFiles = Task.InputFiles ?? Array.Empty<string>();
		}

		public override void Execute()
		{
			Task.BuildIncrementally(Task.InputFiles, inputFiles =>
			{
				var filesInDirectories = Task.InputFiles
					.Where(x => x.StartsWith(Task.RootDirectory));

				File.WriteAllLines(Task.OutputFile, GenerateUnit(filesInDirectories));
				if (!string.IsNullOrWhiteSpace(Task.BaseTypesOutputFile))
				{
					// ReSharper disable once AssignNullToNotNullAttribute
					using (var stream = new StreamReader(typeof(ResourceMapperTask).GetTypeInfo()
						.Assembly.GetManifestResourceStream(
							string.Join(".", typeof(ResourceMapperTask).Namespace, typeof(ResourceDirectoryBase).Namespace, "cs"))))
					{
						var baseTypes = Generator.GeneratedHeader.Concat(stream.ReadToEnd()
							.Split(new[] { Environment.NewLine, "\r\n", "\n" }, StringSplitOptions.None));

						if (Task.BaseTypesOutputFile == Task.OutputFile)
						{
							File.AppendAllLines(Task.BaseTypesOutputFile, baseTypes);
						}
						else
						{
							File.WriteAllLines(Task.BaseTypesOutputFile, baseTypes);
						}
					}
				}

				return new[] { Task.OutputFile, Task.BaseTypesOutputFile };
			});
		}

		private string DirNamespace => Task.TopClassName + "Directories";

		private IEnumerable<string> GenerateUnit(IEnumerable<string> items)
		{
			var hashedItems = items.AsParallel().AsOrdered()
				.Select(item => new KeyValuePair<string[], string>(
					item.Substring(Task.RootDirectory.Length)
						.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
						.ToArray(),
					GetHashForFile(item)));

			return Generator.Flatten(
				Generator.GeneratedHeader,
				new[]
					{
						typeof(IReadOnlyDictionary<string, string>).Namespace,
						typeof(ResourceDirectoryBase).Namespace,
						string.Join(".", Task.Namespace, DirNamespace)
					}
					.Select(x => $"using {x};"),
				new[]
				{
					"// ReSharper disable InconsistentNaming",
					string.Empty,
					$"namespace {Task.Namespace}",
					"{",
				},
				GenerateDirectoryClass(Task.TopClassName, null, hashedItems.ToArray(), 0).Indent(),
				new[]
				{
					"}"
				}
			);
		}


		private IEnumerable<string> GenerateDirectoryClass(string className, string name,
			ICollection<KeyValuePair<string[], string>> items, int level)
		{
			var directories = items
				.Where(x => x.Key.Length > level + 1)
				.GroupBy(x => x.Key[level].ToPascalCaseIdentifier())
				.Select(x => new
				{
					Name = x.Key,
					ClassName = string.Join("_", x.First().Key.Take(level + 1).Select(n => n.ToPascalCaseIdentifier())),
					Identifier = x.Key.ToPascalCaseIdentifier(),
					Files = x.ToArray()
				})
				.ToArray();

			var files = items
				.Where(x => x.Key.Length == level + 1)
				.Select(x => new
				{
					Identifier = x.Key[level].ToPascalCaseIdentifier(),
					Name = x.Key[level],
					Hash = x.Value
				})
				.ToArray();

			var initializations = directories.Select(
					dir =>
						$"directories.Add(nameof({dir.Identifier}), new {dir.ClassName}(this));"
				)
				.Concat(files.Select(
					file =>
						$"files.Add(nameof({file.Identifier}), new {nameof(ResourceFile)}({file.Name.ToVerbatimLiteral()}, {file.Hash.ToVerbatimLiteral()}, this));"
				));

			IEnumerable<string> Comment(string comment, string content)
				=> new[]
				{
					string.Empty,
					"/// <summary>",
					"/// " + comment.XmlEscape(),
					"/// </summary>",
					content
				};

			var declarations = directories.SelectMany(
					dir => Comment(dir.Name,
						$"public {dir.ClassName} {dir.Identifier} => ({dir.ClassName})directories[nameof({dir.Identifier})];")
				)
				.Concat(files.SelectMany(
					file => Comment(file.Name,
						$"public {nameof(IResourceFile)} {file.Identifier} => files[nameof({file.Identifier})];")
				));

			var subDirs = directories
				.SelectMany(dir => GenerateDirectoryClass(dir.ClassName, dir.Name, dir.Files, level + 1));

			if (level == 0)
			{
				subDirs = Generator.Flatten(new[]
					{
						string.Empty,
						$"namespace {DirNamespace}",
						"{",
					},
					subDirs.Indent(),
					new[]
					{
						"}"
					});
			}

			return Generator.Flatten(new[]
				{
					string.Empty,
					$"public sealed class {className} : {nameof(ResourceDirectoryBase)}",
					"{",
				},
				Generator.Flatten(new[]
					{
						string.Empty,
						$"public {className}({nameof(IResourceDirectory)} parentDirectory)",
						$"	: base({name?.ToVerbatimLiteral() ?? "null"}, parentDirectory)",
						"{"
					},
					initializations,
					new[]
					{
						"}",
					},
					declarations
				).Indent(),
				new[]
				{
					"}"
				},
				subDirs
			);
		}

		private static string GetHashForFile(string path)
		{
			using (var sha256 = SHA256.Create())
			{
				using (var readStream = new FileStream(path, FileMode.Open))
				{
					var hash = sha256.ComputeHash(readStream);
					return WebEncoders.Base64UrlEncode(hash);
				}
			}
		}
	}
}