using System;
using System.Collections.Generic;
using System.Linq;
using ResourcePacker.Prototypes;

namespace ResourcePacker
{
	public partial class ResourceMapClass : ResourceMapClassBase

	{
		public string Name { get; }
		public List<KeyValuePair<string, string>> Files { get; } = new List<KeyValuePair<string, string>>();
		public List<KeyValuePair<string, string>> Directories { get; } = new List<KeyValuePair<string, string>>();

		public ResourceMapClass(string name, IEnumerable<string[]> items)
		{
			Name = name;
			foreach (var item in items)
			{
				var newItem = new KeyValuePair<string, string>(item.First().ToPascalCaseIdentifier(), item.First());
				if (item.Length > 1)
					Directories.Add(newItem);
				else
					Files.Add(newItem);
			}
		}

		private IEnumerable<string> Flatten(params string[] )

		//private IEnumerable<string> Constructor

		private void Class(string name,
			IEnumerable<KeyValuePair<string, string>> files,
			IEnumerable<KeyValuePair<string, string>> directories)
		{

			return new[]
				{
					$"public sealed class {name} : ResourceDirectoryBase",
					"{",
				}
				.Concat(new[]
					{
						$"public {name}({nameof(IResourceDirectory)} parentDirectory)",
						$"	: base(nameof({name}), parentDirectory)",
						"{",
					}
					.Concat(files
						.Select(file => new[]
						{
							
						}))
					.Concat(new[]
					{
						"}"
					})
					.Concat()
					)
				.Concat(new[]
				{
					"}"
				});

			return new[]
				{
					$"public {name}({nameof(IResourceDirectory)} parentDirectory)",
					$"	: base(nameof({name}), parentDirectory)",
					"{",
				}
				.Concat(content.Indent())
				.Concat(new[]
				{
					"}"
				});

			var constructor = new string[]
			{
				$"public {name}(IResourceDirectory parentDirectory)",
				"{",
				content,
				"}"
			};
			var list = new List<string>();
			list.Add($"public sealed class { name } : ResourceDirectoryBase");

			return new string[]
			{
				$"public sealed class { name } : ResourceDirectoryBase",
				"{",
				$"public {name}(IResourceDirectory parentDirectory)",




			}
			var 
			List<>
			Console.WriteLine($"public sealed class { name } : ResourceDirectoryBase");

#>public sealed class <#= name #> : ResourceDirectoryBase
			<#+		#>{
			<#+
			PushIndent(Indent);
#>public <#= name #>(IResourceDirectory parentDirectory)
				<#+		    #>	: base(nameof(<#= name #>), parentDirectory)
				<#+		    #>{
				<#+
			PushIndent(Indent);
			foreach (var file in files)
			{
#>files.Add((<#= file.Key #>, new ResourceFile(nameof(<#= file.Key #>), <#= file.Value #>, this));
				<#+
			}
			foreach (var directory in files)
			{
#>directories.Add((<#= directory.Key #>, new <#= directory.Key #>(nameof(<#= directory.Key #>), <#= directory.Value #>, this));
				<#+
			}
#>}

			<#+
			foreach (var file in files)
			{
#>IResourceFile <#= file.Key #> => files[nameof(<#= file.Key #>)];
				<#+		
			}
			foreach (var directory in files)
			{
#><#= directory.Key #> <#= directory.Key #> => directories[nameof(<#= directory.Key #>)];
				<#+		
			}
#>}
			<#+
		}
	}
}