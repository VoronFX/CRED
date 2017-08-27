using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CsCodeGenerator;
using Microsoft.Build.Framework;

namespace CRED.BuildTasks
{
	public abstract class ValueMapper : TaskBase
	{
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

		[DataMember]
		public string ClassName { get; set; }


		public static IEnumerable<string> GenerateValueMap(string @namespace, string className, IEnumerable<ValueMapItem> items)
		{
			var itemsConsts = items
				.SelectMany(item => new[]
					{
						string.Empty,
					}
					.Concat(item.Comment == null || !item.Comment.Any() ? Enumerable.Empty<string>() :
						new[]{
								"/// <summary>"
							}
							.Concat(item.Comment.Select(x =>
								"/// " + x.XmlEscape()
							))
							.Concat(new[]{
								"/// </summary>",
							}))
					.Concat(new[]{
						$"public const string {item.Name.ToPascalCaseIdentifier()} = {item.Value.ToVerbatimLiteral()};"
					}));

			var mapClass = new[]
				{
					string.Empty,
					$"public static class {className}",
					"{",
				}
				.Concat(itemsConsts.Indent())
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

		public class ValueMapItem
		{
			public ValueMapItem(string name, string value, string[] comment)
			{
				Name = name;
				Value = value;
				Comment = comment;
			}

			public string Name { get; }
			public string Value { get; }
			public string[] Comment { get; }
		}

	}
}