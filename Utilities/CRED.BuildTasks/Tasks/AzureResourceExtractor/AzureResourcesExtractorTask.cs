using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using CRED.BuildTasks;
using CsCodeGenerator;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using static CRED.BuildTasks.CssClassesMapper;

namespace CRED.BuildTasks
{
	public sealed partial class AzureResourcesExtractor : TaskBase
	{
		[Required]
		[NormalizeDirectoryPath]
		[DataMember]
		public string RootDirectory { get; set; }

		[Required]
		[ExpandPath]
		[DataMember]
		public string[] Sources { get; set; }

		[Required]
		[NormalizeDirectoryPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OverridesDirectory { get; set; }

		[Required]
		[NormalizeDirectoryPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputExtractedDirectory { get; set; }

		[Required]
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputCssClassesMapFile { get; set; }

		[Required]
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputMissingCssClassesMapFile { get; set; }

		[DataMember]
		public string Namespace { get; set; }

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
						sourceFilePath: $"segoe-ui_cyrillic_{fw}_latest{ft.ext}",
						targetPath: $"Fonts/segoe-ui/cyrillic/{fw}/latest{ft.ext}")
				)).ToArray();


		private static Overrride[] Overrrides { get; } = new[]
			{
				new Overrride(type: Resource.ResType.Style,
					urlToOverride:
					"/Content/Dynamic/UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css?c1=ru&amp;c2=ru",
					sourceFileUrl: CurrentUri +
								   "/Content/Dynamic/UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css?c1=ru&amp;c2=ru",
					sourceFilePath: "UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7.css",
					targetPath: "Styles/LinkHrefStylesheet"),
			}
			.Concat(FontOverrrides)
			.ToArray();


		protected override bool ExecuteWork()
		{
			BuildIncrementally(Sources.Concat(Overrrides.Select(x =>
				Path.GetFullPath(Path.Combine(OverridesDirectory, x.SourceFilePath)))).ToArray(), inputFiles =>
			{

				var parsedResources = Sources
					.AsParallel()
					.AsOrdered()
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
							// TODO url with bracers fix
							resource.Content = Regex.Replace(resource.Content, @"(?is)(?<=url\(').+?(?='\))", match =>
							{
								var newResource = new Resource();
								var resOver = Overrrides.FirstOrDefault(ov => ov.UrlToOverride == match.Value.Trim('\''));
								if (resOver == null)
								{
									missing.Add(match.Value);
									return resource.Content;
								}
								resOver.LoadInto(newResource, OverridesDirectory);
								newResource.Index = resources.Count;
								newResource.EnsureUniq(uniqIdentifiers);
								resources.Add(newResource);
								var newUrl = Path.Combine(OutputExtractedDirectory, newResource.TargetPathFull)
									.Substring(RootDirectory.Length)
									.Replace(Path.DirectorySeparatorChar, '/');

								return $"'{newUrl}'";
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

				var resourcesBase = parsedResources.First();

				// Fill empty styles with other files
				foreach (var resource in resourcesBase.Where(r =>
					r.Type == Resource.ResType.Style
					&& string.IsNullOrWhiteSpace(r.Content)))
				{
					var overlayRes = parsedResources
						.Where(x => x != resourcesBase)
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

				resourcesBase.AsParallel()
					.ForAll(resource => resource.Save(OutputExtractedDirectory));

				GenerateStyleClassesMap(resourcesBase);

				//foreach (string file in Directory.GetFiles(textBox1.Text, "*.html").Select(File.ReadAllText))
				//{
				//}

				return resourcesBase.Where(x => !x.IsEmpty).Select(x => Path.Combine(OutputExtractedDirectory, x.TargetPathFull))
				.Concat(new[] { OutputCssClassesMapFile, OutputMissingCssClassesMapFile });
			});

			return true;
		}



		private void GenerateStyleClassesMap(IEnumerable<Resource> parsedResources)
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

			Task.WaitAll(
				new Action[]
				{
					() =>
					{
						File.WriteAllLines(OutputCssClassesMapFile,
							GenerateCssClassesMap(Namespace, "AzureCssClassesMap",
								classesPacks
									.AsParallel()
									.AsOrdered()
									.SelectMany(x => x.classes
										.Select(c => new {Class = c, File = x.resource.TargetPathFull}))
									.GroupBy(x => x.Class)
									.Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Select(x2 => x2.File)))));
					},
					() =>
					{
						var dummyClasses = Sources
							.AsParallel()
							.AsOrdered()
							.Select(File.ReadAllText)
							.SelectMany(x => Regex.Matches(x, "(?si)(?<=class=\")[-_A-Za-z0-9\\s]+?(?=\")").Cast<Match>())
							.SelectMany(x => x.Value.Split(' '))
							.Select(x => x.Trim())
							.Where(x => !string.IsNullOrWhiteSpace(x))
							.Distinct()
							.Where(x => classesPacks.SelectMany(c => c.classes).All(c => c != x));

						File.WriteAllLines(OutputMissingCssClassesMapFile,
							GenerateCssClassesMap(Namespace, "AzureCssMissingClassesMap",
								dummyClasses.Select(x => new KeyValuePair<string, IEnumerable<string>>(x, Enumerable.Empty<string>()))));

					}
				}.Select(Task.Run).ToArray());
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
						.LoadInto(res, OverridesDirectory);

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