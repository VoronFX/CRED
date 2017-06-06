using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CsCodeGenerator;
using static CsCodeGenerator.Generator;
using Microsoft.AspNetCore.WebUtilities;
using ResourceMapper.Base;
using ResourceMapper.Prototypes;

namespace ResourceMapper
{
	public partial class ResourceMapper
	{
		private ResourceMapperTask Task { get; }

		static ResourceMapper()
		{
			Execute = (task) => new ResourceMapper(task).GenerateMap();
		}

		private ResourceMapper(ResourceMapperTask task)
		{
			Task = task;
			if (string.IsNullOrWhiteSpace(Task.Namespace))
				Task.Namespace = nameof(ResourceMapper);
			if (string.IsNullOrWhiteSpace(Task.OutputFile))
				Task.OutputFile = "ResourceMap.cs";
			if (string.IsNullOrWhiteSpace(Task.TopClassName))
				Task.TopClassName = "ResourceMap";
			Task.InputFiles = Task.InputFiles ?? Array.Empty<string>();
			Task.RootDirectory = IOExtension.NormalizeExpandDirectoryPath(Task.RootDirectory);
		}

		private void GenerateMap()
		{
			var filesInDirectories = Task.InputFiles
				.Select(x => Path.GetFullPath(Path.Combine(Task.RootDirectory, x)))
				.Where(x => x.StartsWith(Task.RootDirectory));

			var outputFile = Path.Combine(Task.RootDirectory, Task.OutputFile);
			IOExtension.EnsureFileDirectoryCreated(outputFile);

			File.WriteAllLines(outputFile, GenerateUnit(filesInDirectories));
			if (!string.IsNullOrWhiteSpace(Task.BaseTypesOutputFile))
			{
				var baseTypesOutputFile = Path.GetFullPath(Path.Combine(Task.RootDirectory, Task.BaseTypesOutputFile));
				IOExtension.EnsureFileDirectoryCreated(baseTypesOutputFile);

				// ReSharper disable once AssignNullToNotNullAttribute

				using (var stream = new StreamReader(Assembly
					.GetExecutingAssembly().GetManifestResourceStream(
					string.Join(".", Assembly.GetExecutingAssembly().GetName().Name, typeof(ResourceDirectoryBase).Namespace, "cs"))))
				{
					var baseTypes = GeneratedHeader.Concat(stream.ReadToEnd()
						.Split(new[] { Environment.NewLine, "\r\n", "\n" }, StringSplitOptions.None));

					if (baseTypesOutputFile == outputFile)
					{
						File.AppendAllLines(baseTypesOutputFile, baseTypes);
					}
					else
					{
						File.WriteAllLines(baseTypesOutputFile, baseTypes);
					}
				}
			}
		}

		private IEnumerable<string> GenerateUnit(IEnumerable<string> items)
		{
			var hashedItems = items.AsParallel().AsOrdered()
				.Select(item => new KeyValuePair<string[], string>(
					item.Substring(Task.RootDirectory.Length)
						.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
						.ToArray(),
					GetHashForFile(item)));

			return Flatten(
					GeneratedHeader,
					new[] { typeof(IReadOnlyDictionary<string, string>).Namespace, typeof(ResourceDirectoryBase).Namespace }
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



		private IEnumerable<string> GenerateDirectoryClass(string className, string name, ICollection<KeyValuePair<string[], string>> items, int level)
		{
			var scope = level == 0 ? "Directories." : null;

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
						$"directories.Add(nameof({dir.Identifier}), new {scope + dir.ClassName}(this));"
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
						$"public {scope + dir.ClassName} {dir.Identifier} => ({scope + dir.ClassName})directories[nameof({dir.Identifier})];")
				)
				.Concat(files.SelectMany(
					file => Comment(file.Name,
						$"public {nameof(IResourceFile)} {file.Identifier} => files[nameof({file.Identifier})];")
				));

			var subDirs = directories
				.SelectMany(dir => GenerateDirectoryClass(dir.ClassName, dir.Name, dir.Files, level + 1));

			if (level == 0)
			{
				subDirs = Flatten(new[]
					{
						string.Empty,
						$"namespace Directories",
						"{",
					},
					subDirs.Indent(),
					new[]
					{
						"}"
					});
			}

			return Flatten(new[]
				{
					string.Empty,
					$"public sealed class {className} : {nameof(ResourceDirectoryBase)}",
					"{",
				},
				Flatten(new[]
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