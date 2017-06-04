using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsCodeGenerator;
using ResourceMapper;

namespace AzureResourcesExtractor
{
	public partial class AzureResourcesExtractor
	{
		private static Uri CurrentUri { get; } = new Uri("http://portal.azure.com/");

		private static readonly string[] FontWeights =
		{
			"normal", "light", "semilight", "semibold", "bold"
		};

		private static readonly (string uri, string ext, Resource.ResType type)[] FontTypes =
		{
			(".eot", ".eot", Resource.ResType.FontEot),
			(".eot?#iefix", "_iefix.eot", Resource.ResType.FontEot),
			(".woff", ".woff", Resource.ResType.FontWoff),
			(".ttf", ".ttf", Resource.ResType.FontTtf),
			(".svg#web", ".svg", Resource.ResType.FontSvg)
		};

		private static Overrride[] FontOverrrides { get; } =
			FontWeights.SelectMany(fw =>
				FontTypes.Select(ft =>
					new Overrride(type: ft.type,
						urlToOverride: $"../fonts/segoe-ui/cyrillic/{fw}/latest{ft.uri}",
						sourceFileUrl: CurrentUri + $"Content/fonts/segoe-ui/cyrillic/{fw}/latest{ft.uri}",
						sourceFilePath: $"Overrides/segoe-ui_cyrillic_{fw}_latest{ft.ext}",
						targetPath: $"Fonts/segoe-ui/cyrillic/{fw}/latest{ft.ext}")
				)).ToArray();


		private static Overrride[] Overrrides { get; } = new[]
			{
				new Overrride(type: Resource.ResType.Style,
					urlToOverride:
					"/Content/Dynamic/UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css?c1=ru&amp;c2=ru",
					sourceFileUrl: CurrentUri +
								   "/Content/Dynamic/UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css?c1=ru&amp;c2=ru",
					sourceFilePath: "Overrides/UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css",
					targetPath: "Styles/LinkHrefStylesheet"),
			}
			.Concat(FontOverrrides)
			.ToArray();

		public AzureResourcesExtractorTask Task { get; }

		static AzureResourcesExtractor()
		{
			Execute = (task) => new AzureResourcesExtractor(task).ExtractAzureResources();
		}

		private AzureResourcesExtractor(AzureResourcesExtractorTask task)
		{
			Task = task;
			Task.SourcesDirectory = IOExtension.NormalizeExpandDirectoryPath(Task.SourcesDirectory);
			Task.OutputDirectory = IOExtension.NormalizeExpandDirectoryPath(Task.OutputDirectory);
			Task.OutputMapsDirectory = IOExtension.NormalizeExpandDirectoryPath(Task.OutputMapsDirectory);
			Task.RootDirectory = IOExtension.NormalizeExpandDirectoryPath(Task.RootDirectory);
		}

		private void ExtractAzureResources()
		{

			var parsedResources = Directory.GetFiles(Task.SourcesDirectory, "*.html")
				.Select(File.ReadAllText)
				.Select(x =>
				{
					var head = Regex.Match(x, "(?is)(?<=<head>).*?(?=</head>)").Value;
					var resources = new List<Resource>();
					var uniqIdentifiers = new HashSet<string>();
					var tags = Regex.Matches(head,
						"<(?is)(?:style|svg)[^<>]*?>.*?</(?:style|svg)>|<link[^<>]*?type=\"text/css\"[^<>]*?>");

					// Extract styles and svgs
					foreach (var tag in tags.Cast<Match>().Select(t => t.Value))
					{
						var resource = ParseAzurePage(tag);
						resource.Index = resources.Count;
						resource.EnsureUniq(uniqIdentifiers);
						resources.Add(resource);
					}

					var missing = new List<string>();

					// Replace missing resources with overrides
					foreach (var resource in resources
						.Where(r => r.Type == Resource.ResType.Style)
						.ToArray())
						resource.Content = Regex.Replace(resource.Content, @"(?is)(?<=url\(').+?(?='\))", match =>
						{
							var newResource = new Resource();
							var resOver = Overrrides.FirstOrDefault(ov => ov.UrlToOverride == match.Value);
							if (resOver == null)
							{
								missing.Add(match.Value);
								return resource.Content;
							}
							resOver.LoadInto(newResource, Task.SourcesDirectory);
							newResource.Index = resources.Count;
							newResource.EnsureUniq(uniqIdentifiers);
							resources.Add(newResource);
							return Path.Combine(Task.OutputDirectory, newResource.TargetPathFull)
										.Substring(Task.RootDirectory.Length)
										.Replace(Path.DirectorySeparatorChar, '/');
						});

					// Throw all missing resources in one exception
					if (missing.Any())
						throw new Exception("Missing overrides for such resources: "
											+ Environment.NewLine + string.Join(Environment.NewLine, missing));

					//TODO: Prettify extracted files some day?

					// Format
					//Parallel.ForEach(resources, resource =>
					//{
					//	switch (resource.Type)
					//	{
					//		case Resource.ResType.Svg:
					//			using (Document doc = Document.FromString($"<html><head></head><body>{resource.Content}</body><html>"))
					//			{
					//				doc.OutputBodyOnly = AutoBool.Yes;
					//				//doc.Quiet = true;
					//				doc.CleanAndRepair();
					//				resource.Content = doc.Save();
					//			}
					//			break;
					//		case Resource.ResType.Style:
					//			resource.Content = FormatCss(resource.Content);
					//			break;
					//	}
					//});

					return resources;
				}).ToArray();

			var resourcesBase = parsedResources.First().ToArray();

			// Fill empty styles with other files
			foreach (var resource in resourcesBase.Where(r =>
				r.Type == Resource.ResType.Style
				&& string.IsNullOrWhiteSpace(r.Content)))
			{
				var overlayRes = parsedResources
					.Skip(1)
					.SelectMany(x => x)
					.FirstOrDefault(ro => ro.IdentifierFull == resource.IdentifierFull
										  && !string.IsNullOrWhiteSpace(ro.Content));
				if (overlayRes != null)
					resource.Content = overlayRes.Content;
			}

			// Encapsulate some styles
			//foreach (var resource in resourcesBase
			//	.Where(r =>r.Type == Resource.ResType.Style))
			//{
			//	resource.Content = Regex.Replace(resource.Content, 
			//		"(?<=[^a-zA-Z0-9-_])(body|html)(?=[^a-zA-Z0-9-_])", "div.azure-$1");
			//}

			if (Directory.Exists(Task.OutputDirectory))
				Directory.Delete(Task.OutputDirectory, true);

			Parallel.ForEach(resourcesBase,
				resource => resource.Save(Task.OutputDirectory));

			GenerateStyleClassesMap(resourcesBase);

			//foreach (string file in Directory.GetFiles(textBox1.Text, "*.html").Select(File.ReadAllText))
			//{
			//}
		}

		private IEnumerable<string> GenerateCssClassesMap(string className, IEnumerable<KeyValuePair<string, IEnumerable<string>>> classes)
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

			return Generator.GeneratedHeader()
				.Concat(new[]
				{
					"// ReSharper disable InconsistentNaming",
					string.Empty,
					$"namespace {Task.Namespace}",
					"{",
				})
				.Concat(mapClass.Indent())
				.Concat(new[]
				{
					"}"
				});
		}

		private void GenerateStyleClassesMap(Resource[] parsedResources)
		{
			var classesPacks = parsedResources.Where(resource => resource.Type == Resource.ResType.Style)
				.Select(r => new
				{
					resource = r,
					classes = Regex.Matches(Regex.Replace(r.Content, @"(?is)\{.*?\}", " "), @"(?is)\.[A-Z_a-z0-9-]+")
						.Cast<Match>()
						.Select(rgx => rgx.Value.Substring(1))
						.Distinct()
						.ToList()
				}).ToArray();

			Directory.CreateDirectory(Task.OutputMapsDirectory);

			File.WriteAllLines(Path.Combine(Task.OutputMapsDirectory, "AzureCssClassesMap.cs"),
				GenerateCssClassesMap("AzureCssClassesMap",
					classesPacks
						.SelectMany(x => x.classes
							.Select(c => new { Class = c, File = x.resource.TargetPathFull }))
						.GroupBy(x => x.Class)
						.Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Select(x2 => x2.File)))));

			var dummyClasses = Directory.GetFiles(Task.SourcesDirectory, "*.html")
				.Select(File.ReadAllText)
				.SelectMany(x => Regex.Matches(x, "(?si)(?<=class=\")[-_A-Za-z0-9\\s]+?(?=\")").Cast<Match>())
				.SelectMany(x => x.Value.Split(' '))
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct()
				.Where(x => classesPacks.SelectMany(c => c.classes).All(c => c != x));

			File.WriteAllLines(Path.Combine(Task.OutputMapsDirectory, "AzureCssMissingClassesMap.cs"),
				GenerateCssClassesMap("AzureCssMissingClassesMap",
					dummyClasses.Select(x => new KeyValuePair<string, IEnumerable<string>>(x, Enumerable.Empty<string>()))));
		}

		private static string FormatCss(string input)
			=> input
				//.Replace(".", $"{Environment.NewLine}.")
				.Replace("}#", $"}}{Environment.NewLine}#")
				.Replace(";", $";{Environment.NewLine}")
				.Replace("{", $"{{{Environment.NewLine}")
				.Replace("}", $"}}{Environment.NewLine}");

		private Resource ParseAzurePage(string eltext)
		{
			var res = new Resource()
			{
				Content = Regex.Match(eltext, "(?is)(?<=<[^<>]*?>).*(?=<[^<>]*?>)").Value
			};

			var tag = Regex.Match(eltext, "(?is)(?<=<)\\w+?(?=>|(?:\\s+?[^<]*?>))").Value.ToLowerInvariant();

			string attr(string tagName, string attrName) =>
				Regex.Match(eltext, $"(?is)(?<=<{tagName}\\s+?[^<>]*?{attrName}=\").*?(?=\"[^<]*?>)").Value;

			switch (tag)
			{
				case "link":

					var href = attr("link", "href");

					Overrrides
						.First(ov => ov.UrlToOverride == href)
						.LoadInto(res, Task.SourcesDirectory);

					break;
				case "style":

					var extensionwindowid = attr("style", "data-extensionwindowid");
					var styleClass = attr("style", "class");

					if (!string.IsNullOrWhiteSpace(extensionwindowid))
					{
						res.TargetPath = "ExtensionWindow/" + extensionwindowid;
						res.Comment = extensionwindowid;
					}
					else if (!string.IsNullOrWhiteSpace(styleClass))
					{
						res.TargetPath = styleClass;
						res.Comment = styleClass;
					}
					else
					{
						res.TargetPath = "UnnamedStylesheet";
						res.Comment = "Style hadn't any identifier";
					}

					res.TargetPath = "Styles/" + res.TargetPath;
					res.Type = Resource.ResType.Style;

					break;
				case "svg":

					var id = attr("symbol", "id");

					if (string.IsNullOrWhiteSpace(id))
						throw new Exception("Svg symbol id not found");

					res.Comment = id;
					res.TargetPath = "Svg/" + id;
					res.Type = Resource.ResType.Svg;

					break;
				default:
					throw new Exception("Parse exception! Uknown element matched");
			}

			return res;
		}
	}
}