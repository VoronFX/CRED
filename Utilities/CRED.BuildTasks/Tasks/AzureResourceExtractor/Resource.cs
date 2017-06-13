using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsCodeGenerator;

namespace CRED.BuildTasks.Tasks.AzureResourceExtractor
{
	public partial class AzureResourcesExtractorTask
	{
		private class Resource
		{
			private string targetPath;
			public int Index { get; set; }
			public string Content { get; set; }
			public byte[] BinaryContent { get; set; }

			public string IdentifierBase => Regex.Replace(TargetPath, "[^A-Za-z0-9]", "_");
			public string IdentifierFull => Regex.Replace(TargetPathFull, "[^A-Za-z0-9]", "_");

			public string TargetPath
			{
				get => targetPath;
				set
				{
					value = Regex.Replace(value, "(?i)\\.css$", "");
					value = Regex.Replace(value, @"[^A-Za-z0-9\.\\/]", "_");
					targetPath = value;
				}
			}

			public string TargetPathFull => targetPath + Extension;

			public string Comment { get; set; }
			public ResType Type { get; set; }

			public string Extension
			{
				get
				{
					switch (Type)
					{
						case ResType.Svg:
							return ".svg";
						case ResType.Style:
							return ".css";
						case ResType.FontEot:
							return ".eof";
						case ResType.FontWoff:
							return ".woff";
						case ResType.FontTtf:
							return ".ttf";
						case ResType.FontSvg:
							return ".svg";
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			public void EnsureUniq(ISet<string> takenIdentifiers)
			{
				int i = 1;
				string postfix = string.Empty;
				while (takenIdentifiers.Contains(IdentifierBase + postfix))
				{
					postfix = $"_{i}";
					i++;
				}
				TargetPath += postfix;
				takenIdentifiers.Add(IdentifierBase);
			}

			public enum ResType
			{
				Svg,
				Style,
				FontEot,
				FontWoff,
				FontTtf,
				FontSvg
			}

			public bool IsEmpty
			{
				get
				{
					switch (Type)
					{
						case ResType.Svg:
						case ResType.Style:
							return string.IsNullOrWhiteSpace(Content);
						case ResType.FontEot:
						case ResType.FontWoff:
						case ResType.FontTtf:
						case ResType.FontSvg:
							return BinaryContent == null || BinaryContent.Length == 0;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}

			public void Save(string outputDirectory)
			{
				if (IsEmpty) return;

				var filePath = Path.Combine(outputDirectory, TargetPathFull);
				IOExtension.EnsureFileDirectoryCreated(filePath);

				switch (Type)
				{
					case ResType.Svg:
					case ResType.Style:
						File.WriteAllText(filePath, Content);
						break;
					case ResType.FontEot:
					case ResType.FontWoff:
					case ResType.FontTtf:
					case ResType.FontSvg:
						File.WriteAllBytes(filePath, BinaryContent);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

		}
	}
}