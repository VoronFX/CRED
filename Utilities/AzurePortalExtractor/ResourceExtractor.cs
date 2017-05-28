using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TidyManaged;
using CsCodeGenerator;

namespace AzurePortalExtractor
{
	public partial class Extractor
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
					targetPath: "LinkHrefStylesheet"),
			}
			.Concat(FontOverrrides)
			.ToArray();

		public void ExtractAzureResources(string sourceDirectory, string wwwrootDirectory, string relativeOutputDirectory)
		{
			var outputDirectory = Path.Combine(wwwrootDirectory, relativeOutputDirectory);

			var parsedResources = Directory.GetFiles(sourceDirectory, "*.html")
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
						var resource = ParseAzurePage(tag, sourceDirectory);
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
							resOver.LoadInto(newResource, sourceDirectory);
							newResource.Index = resources.Count;
							newResource.EnsureUniq(uniqIdentifiers);
							resources.Add(newResource);
							return $@"{Path.Combine(relativeOutputDirectory, newResource.TargetPathFull).Replace(@"\", "/")}";
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

			Parallel.ForEach(resourcesBase,
				resource => resource.Save(outputDirectory));

			GenerateDefinitions(relativeOutputDirectory, outputDirectory, resourcesBase);
			GenerateRequireAllStyles(outputDirectory, resourcesBase);
			GenerateStyleClassesMap(outputDirectory, sourceDirectory, resourcesBase);

			//foreach (string file in Directory.GetFiles(textBox1.Text, "*.html").Select(File.ReadAllText))
			//{
			//}
		}

		private static readonly string GeneratedXmlNamespace = "AzurePortal";

		private static readonly string[] GeneratedXmlComment =
		{
			$"Content genereated by {nameof(AzurePortalExtractor)}",
			"ReSharper disable InconsistentNaming"
		};

		private static void GenerateDefinitions(string relativeOutputDirectory, string outputDirectory,
			Resource[] parsedResources)
		{
			void GenDef(string name, IEnumerable<Resource> resources)
			{
				File.WriteAllLines(outputDirectory + $"Generated{name}Definitions.cs",
					Generator.Namespace(
						name: GeneratedXmlNamespace,
						comment: GeneratedXmlComment,
						imports: new string[] { },
						content: Generator.Class(
							name: "Definitions",
							comment: new string[] { },
							@static: true,
							partial: true,
							@public: true,
							@sealed: false,
							content: Generator.Class(
								name: $"{name}",
								comment: new string[] { },
								@static: true,
								partial: false,
								@public: true,
								@sealed: false,
								content: Generator.Property(
										name: $"{name}Directory",
										value: $"@\"{relativeOutputDirectory.Replace(@"\", "/")}\"",
										comment: new[] { $"Path to {name} folder" },
										@const: true,
										@static: false,
										field: true,
										@public: true,
										@readonly: false,
										type: "string")
									.Concat(resources
										.SelectMany(r =>
											Generator.Property(
												name: r.IdentifierFull,
												value: $"{name}Directory + @\"{r.TargetPathFull.Replace(@"\", "/")}\"",
												comment: new[] { r.Comment },
												@const: true,
												@static: false,
												field: true,
												@public: true,
												@readonly: false,
												type: "string")
										)
									)
							)
						)
					)
				);
			}

			GenDef("Styles", parsedResources.Where(resource => resource.Type == Resource.ResType.Style));
			GenDef("Svg", parsedResources.Where(resource => resource.Type == Resource.ResType.Svg));
			GenDef("Fonts", parsedResources.Where(resource =>
				resource.Type == Resource.ResType.FontWoff ||
				resource.Type == Resource.ResType.FontTtf ||
				resource.Type == Resource.ResType.FontEot ||
				resource.Type == Resource.ResType.FontSvg));
		}



		private static void GenerateStyleClassesMap(string outputDirectory, string sourceDirectory, Resource[] parsedResources)
		{
			var classesPacks = parsedResources.Where(resource => resource.Type == Resource.ResType.Style)
				.Select(r => new
				{
					resource = r,
					classes = Regex.Matches(Regex.Replace(r.Content, @"(?is)\{.*?\}", " "), @"(?is)\.[A-Z_a-z0-9-]+")
						.Cast<Match>()
						.Select(rgx => new
						{
							className = rgx.Value.Substring(1),
							identifier = rgx.Value.ToPascalCaseIdentifier()
						})
						.Distinct()
						.ToList()
				}).ToArray();

			foreach (var classesPack in classesPacks)
			{
				foreach (var styleClass in classesPack.classes)
				{
					foreach (var otherPack in classesPacks.Where(x => x != classesPack))
					{
						if (otherPack.classes.Contains(styleClass))
							otherPack.classes.Remove(styleClass);
					}
				}
			}

			File.WriteAllLines(outputDirectory + $"GeneratedStyleClassesMap.cs",
				Generator.Namespace(
					name: GeneratedXmlNamespace,
					comment: GeneratedXmlComment,
					imports: new string[] { },
					content: classesPacks.SelectMany(cp =>
						Generator.Class(
							name: "StyleClassesMap",
							comment: new[]
							{
								cp.resource.IdentifierFull,
								cp.resource.TargetPathFull,
								cp.resource.Comment
							},
							@static: false,
							partial: true,
							@public: true,
							@sealed: true,
							content: cp.classes.SelectMany(r =>
								Generator.Property(
									name: r.identifier,
									value: $"@\"{r.className}\"",
									comment: new string[] { },
									@const: false,
									@static: false,
									field: false,
									@public: true,
									@readonly: true,
									type: "string"))
						)
					)
				)
			);

			var dummyStyleClasses = Directory.GetFiles(sourceDirectory, "*.html")
				.Select(File.ReadAllText)
				.SelectMany(x => Regex.Matches(x, "(?si)(?<=class=\")[-_A-Za-z0-9\\s]+?(?=\")").Cast<Match>())
				.SelectMany(x => x.Value.Split(' '))
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Distinct()
				.Select(x => new { className = x, identifier = x.ToPascalCaseIdentifier() })
				.Where(x => classesPacks.SelectMany(c => c.classes).All(c => c.identifier != x.identifier));

			File.WriteAllLines(outputDirectory + $"GeneratedDummyClassesMap.cs",
				Generator.Namespace(
					name: GeneratedXmlNamespace,
					comment: GeneratedXmlComment,
					imports: new string[] { },
					content: 
					Generator.Class(
							name: "DummyClassesMap",
							comment: new[]{ "Classes that has no associated style" },
							@static: false,
							partial: false,
							@public: true,
							@sealed: true,
							content: dummyStyleClasses.SelectMany(r =>
								Generator.Property(
									name: r.identifier,
									value: $"@\"{r.className}\"",
									comment: new string[] { },
									@const: false,
									@static: false,
									field: false,
									@public: true,
									@readonly: true,
									type: "string"))
						)
					
				)
			);
		}

		private static void GenerateRequireAllStyles(string outputDirectory,
			Resource[] parsedResources)
		{
			File.WriteAllLines(outputDirectory + $"GeneratedRequireAllStyles.cs",
				Generator.Namespace(
					name: GeneratedXmlNamespace,
					comment: GeneratedXmlComment,
					imports: new[] { "CRED", $"static {GeneratedXmlNamespace}.Definitions.Styles" },
					content: parsedResources
						.Where(r => r.Type == Resource.ResType.Style)
						.Select(r => $"[RequireResource({r.IdentifierFull})]")
						.Concat(new[]
						{
							"public static class RequireAllStyles { }", string.Empty
						})
				)
			);
		}

		private static string FormatCss(string input)
			=> input
				//.Replace(".", $"{Environment.NewLine}.")
				.Replace("}#", $"}}{Environment.NewLine}#")
				.Replace(";", $";{Environment.NewLine}")
				.Replace("{", $"{{{Environment.NewLine}")
				.Replace("}", $"}}{Environment.NewLine}");

		private Resource ParseAzurePage(string eltext, string sourceDirectory)
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
						.LoadInto(res, sourceDirectory);

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