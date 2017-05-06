using System;
using System.IO;
using System.Net.Http;

namespace AzurePortalExtractor
{
	public partial class Extractor
	{
		private class Overrride
		{
			public Overrride(string urlToOverride, string sourceFileUrl,
				string sourceFilePath, string targetPath, Resource.ResType type)
			{
				UrlToOverride = urlToOverride;
				SourceFileUrl = sourceFileUrl;
				SourceFilePath = sourceFilePath;
				TargetPath = targetPath;
				Type = type;
			}

			public string UrlToOverride { get; }
			public string SourceFileUrl { get; }
			public string SourceFilePath { get; }
			public string TargetPath { get; }
			public Resource.ResType Type { get; }

			public void LoadInto(Resource resource, string sourceDirectory)
			{
				resource.TargetPath = TargetPath;
				resource.Type = Type;
				resource.Comment = UrlToOverride;

				var filePath = Path.Combine(sourceDirectory, SourceFilePath);

				if (File.Exists(filePath))
				{
					switch (Type)
					{
						case Resource.ResType.Svg:
						case Resource.ResType.Style:
							resource.Content = File.ReadAllText(filePath);
							break;
						case Resource.ResType.FontEot:
						case Resource.ResType.FontWoff:
						case Resource.ResType.FontTtf:
						case Resource.ResType.FontSvg:
							resource.BinaryContent = File.ReadAllBytes(filePath);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					var uri = new Uri(SourceFileUrl, UriKind.RelativeOrAbsolute);
					if (!uri.IsAbsoluteUri)
						uri = CurrentUri.MakeRelativeUri(uri);

					switch (Type)
					{
						case Resource.ResType.Svg:
						case Resource.ResType.Style:
							resource.Content = new HttpClient().GetStringAsync(uri).Result;
							File.WriteAllText(filePath, resource.Content);
							break;
						case Resource.ResType.FontEot:
						case Resource.ResType.FontWoff:
						case Resource.ResType.FontTtf:
						case Resource.ResType.FontSvg:
							resource.BinaryContent = new HttpClient().GetByteArrayAsync(uri).Result;
							File.WriteAllBytes(filePath, resource.BinaryContent);
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
		}
	}
}