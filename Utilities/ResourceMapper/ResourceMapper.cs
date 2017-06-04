using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CsCodeGenerator;
using Microsoft.AspNetCore.WebUtilities;
using ResourceMapper.Prototypes;

namespace ResourceMapper
{
	public class ResourceMapper
	{
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

		private ResourceMapperTask Task { get; }

		public static void GenerateMap(ResourceMapperTask task)
			=> new ResourceMapper(task).GenerateMap();

		private void GenerateMap()
		{
			var filesInDirectories = Task.InputFiles
				.Select(x => Path.GetFullPath(Path.Combine(Task.RootDirectory, x)))
				.Where(x => x.StartsWith(Task.RootDirectory));

			var outputFile = Path.Combine(Task.RootDirectory, Task.OutputFile);
			IOExtension.EnsureFileDirectoryCreated(outputFile);
			File.WriteAllLines(outputFile, GenerateUnit(filesInDirectories));
		}

		private IEnumerable<string> GenerateUnit(IEnumerable<string> items)
		{
			var additionalContent = @"
				public interface IResourceDirectory
				{
					IReadOnlyDictionary<string, IResourceFile> Files { get; }
					IReadOnlyDictionary<string, IResourceDirectory> Directories { get; }
					string Name { get; }
					IResourceDirectory ParentDirectory { get; }
				}

				public interface IResourceFile
				{
					string Name { get; }
					string Hash { get; }
					IResourceDirectory ContainingDirectory { get; }
				}

				internal sealed class ResourceFile : IResourceFile
				{
					internal ResourceFile(string name, string hash, IResourceDirectory containingDirectory)
					{
						Name = name;
						Hash = hash;
						ContainingDirectory = containingDirectory;
					}

					public string Name { get; }
					public string Hash { get; }
					public IResourceDirectory ContainingDirectory { get; }
				}

				public abstract class ResourceDirectoryBase : IResourceDirectory
				{
					protected internal readonly Dictionary<string, IResourceFile> files
						= new Dictionary<string, IResourceFile>();
					protected internal readonly Dictionary<string, IResourceDirectory> directories
						= new Dictionary<string, IResourceDirectory>();
					protected internal readonly IResourceDirectory parentDirectory;
					protected internal readonly string name;

					protected internal ResourceDirectoryBase(string name, IResourceDirectory parentDirectory)
					{
						this.parentDirectory = parentDirectory;
						this.name = name;
					}

					string IResourceDirectory.Name => name;

					IResourceDirectory IResourceDirectory.ParentDirectory => parentDirectory;

					IReadOnlyDictionary<string, IResourceFile> IResourceDirectory.Files => files;

					IReadOnlyDictionary<string, IResourceDirectory> IResourceDirectory.Directories => directories;

				}
			".UnindentVerbatim();

			var hashedItems = new ConcurrentBag<KeyValuePair<string[], string>>();

			Parallel.ForEach(items, item =>
				hashedItems.Add(new KeyValuePair<string[], string>(
					item.Substring(Task.RootDirectory.Length)
						.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
						.ToArray(),
					GetHashForFile(item)
				))
			);

			return Generator.GeneratedHeader()
				.Concat(new[] { typeof(IReadOnlyDictionary<string, string>).Namespace }.Select(x => $"using {x};"))
				.Concat(new[]
				{
					"// ReSharper disable InconsistentNaming",
					string.Empty,
					$"namespace {Task.Namespace}",
					"{",
				})
				.Concat(GenerateDirectoryClass(Task.TopClassName, null, hashedItems.ToArray(), 0).Indent())
				.Concat(additionalContent.Indent())
				.Concat(new[]
				{
					"}"
				});
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

			var constructorContent =
				files.SelectMany(file => new[]
					{
						$"files.Add(nameof({file.Identifier}), new {nameof(ResourceFile)}({file.Name.ToVerbatimLiteral()}, {file.Hash.ToVerbatimLiteral()}, this));"
					})
					.Concat(directories.SelectMany(directory => new[]
					{
						$"directories.Add(nameof({directory.Identifier}), new {scope+directory.ClassName}(this));"
					}));

			var fileProperties = files
				.SelectMany(file => new[]
				{
					string.Empty,
					"/// <summary>",
					"/// " + file.Name.XmlEscape(),
					"/// </summary>",
					$"public {nameof(IResourceFile)} {file.Identifier} => files[nameof({file.Identifier})];"
				});

			var dirProperties = directories
				.SelectMany(dir => new[]
				{
					string.Empty,
					"/// <summary>",
					"/// " + dir.Name.XmlEscape(),
					"/// </summary>",
					$"public {scope+dir.ClassName} {dir.Identifier} => ({scope+dir.ClassName})directories[nameof({dir.Identifier})];",
				});

			var classContent = new[]
				{
					string.Empty,
					$"public {className}({nameof(IResourceDirectory)} parentDirectory)",
					$"	: base({name?.ToVerbatimLiteral() ?? "null"}, parentDirectory)",
					"{"
				}
				.Concat(constructorContent.Indent())
				.Concat(new[]
				{
					"}",
				})
				.Concat(fileProperties)
				.Concat(dirProperties);

			var subDirs = directories.SelectMany(x =>
				GenerateDirectoryClass(x.ClassName, x.Name, x.Files, level + 1));

			if (level == 0)
			{
				subDirs = new[]
					{
						string.Empty,
						$"namespace Directories",
						"{",
					}
					.Concat(subDirs.Indent())
					.Concat(new[]
					{
						"}"
					});
			}

			return new[]
				{
					string.Empty,
					$"public sealed class {className} : {nameof(ResourceDirectoryBase)}",
					"{",
				}
				.Concat(classContent.Indent())
				.Concat(new[]
				{
					"}"
				})
				.Concat(subDirs);
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