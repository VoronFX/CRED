using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ResourceMapper;

namespace AzureResourcesExtractor
{
	public partial class Extractor
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

			public void Save(string outputDirectory)
			{
				var filePath = Path.Combine(outputDirectory, TargetPathFull);
				IOExtension.EnsureFileDirectoryCreated(filePath);

				switch (Type)
				{
					case Resource.ResType.Svg:
					case Resource.ResType.Style:
						if (!string.IsNullOrWhiteSpace(Content))
							File.WriteAllText(filePath, Content);
						break;
					case Resource.ResType.FontEot:
					case Resource.ResType.FontWoff:
					case Resource.ResType.FontTtf:
					case Resource.ResType.FontSvg:
						if (BinaryContent != null && BinaryContent.Length > 0)
							File.WriteAllBytes(filePath, BinaryContent);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

			}

		}
	}
}