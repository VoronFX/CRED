using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
// ReSharper disable InconsistentNaming

namespace AzurePortalExtractor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private const string cssNamesRegex = "(?<=class=\")[^\"].+?css(?:\")";

		private void ExtractSvgToAngularTemplate_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Input.Text))
					Input.Text = Clipboard.GetText();

				var FxSymbolContainer = Regex.Match(Input.Text, "(?<=<div\\s+?id=\"FxSymbolContainer\">).+?(?=</div>)").Value;
				var svgs = Regex.Split(FxSymbolContainer, "<svg>")
					.Select(s => Regex.Replace(
						s.Replace("<svg>", "")
						.Replace("</svg>", "")
						.Replace("<symbol", "<svg")
						.Replace("</symbol>", "</svg>")
						.Replace("<defs>", "")
						.Replace("</defs>", ""),
						"id=\"(?<id>.+?)\"", "*ngIf=\"svgName=='${id}'\""));
				Input.Text = string.Join("\r\n", svgs);
				//.Replace("class role=\"presentation\"", "role=\"img\"")
				Input.SelectAll();
				Clipboard.SetText(Input.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Input.Text))
					Input.Text = Clipboard.GetText();

				var list = new List<string>();
				foreach (Match match in Regex.Matches(Input.Text, "fxs(?<name>[-_][-_a-z]+?)[^-_a-z]"))
				{
					var s = Regex.Replace(match.Groups["name"].Value, "-[a-z]", match1 => match1.Value.ToUpper());
					s = Regex.Replace(s, "[-_]", "");
					//s = Regex.Replace(s, "fxs[A-Z]", "");
					//list.Add(s);
					list.Add($@"public static string {s} => Compose(nameof({s}));");
				}
				Input.Text = string.Join("\r\n", list.Distinct().OrderBy(x => x));
				Input.SelectAll();
				Clipboard.SetText(Input.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		static Uri currentUri = new Uri("http://portal.azure.com/");

		(string downlodHref,
		string replaceHref,
		string sourcePath,
		string targetPath,
		string extension)[] overrides = new[]
			{
				(currentUri + "../fonts/segoe-ui/cyrillic/normal/latest.eot",
				"../fonts/segoe-ui/cyrillic/normal/latest.eot",
				"Overrides/segoe-ui_cyrillic_normal_latest.eot",
				"Fonts/segoe-ui/cyrillic/normal/latest", ".eot"),

				(currentUri + "../fonts/segoe-ui/cyrillic/normal/latest.eot",
				"../fonts/segoe-ui/cyrillic/normal/latest.eot",
				"Overrides/segoe-ui_cyrillic_normal_latest.eot",
				"LinkHrefStylesheet", ".css"),
			};


		private void ExtractFromAzureSourcesBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var SourceDirectory = textBox1.Text;
				var WwwrootDirectory = textBox2.Text;
				var RelativeOutputDirectory = textBox3.Text;
				var OutputDirectory = Path.Combine(WwwrootDirectory, RelativeOutputDirectory);

				//var _Content_Dynamic_UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7_css_c1_ru_c2_ru_ "df";
				//Viva_Controls__Controls__Visualization__Viva_Map_css = Viva_Controls__Controls__Visualization__Viva_Map_css;
				var parsedResources = Directory.GetFiles(textBox1.Text, "*.html")
					.Select(File.ReadAllText)
					.Select(x =>
					{
						var head = Regex.Match(x, "(?i)(?<=<head>).*?(?=</head>)").Value;
						var takenIdentifiers = new HashSet<string>();

						return Regex.Matches(head, "<(?:style|svg)[^<>]*?>.*?</(?:style|svg)>|<link[^<>]*?type=\"text/css\"[^<>]*?>")
							.Cast<Match>()
							.Select(el => ParseAzurePage(el, takenIdentifiers, OutputDirectory));

					}).ToArray();

				foreach (var valueTuple in parsedResources[0])
				{
					
				}

				var resDefinitions = new List<(string)>();

				foreach (var valueTuple in parsedResources[0])
				{
					foreach (Match match in 
						Regex.Matches(valueTuple.Content, @"(?i)(?<=url\(').+?(?='\))")
						.Cast<Match>())
					{
						var resOverride = overrides.First(ov => ov.replaceHref == match.Value);

						var linkOverrideFilePath = Path.Combine(OutputDirectory, resOverride.sourcePath);
						if (File.Exists(linkOverrideFilePath))
						{
							Content = File.ReadAllBytes(linkOverrideFilePath);
						}
						else
						{
							var uri = new Uri(href, UriKind.RelativeOrAbsolute);
							if (!uri.IsAbsoluteUri)
								uri = currentUri.MakeRelativeUri(uri);

							Content = new HttpClient().GetStringAsync(uri).Result;
							File.WriteAllText(linkOverrideFilePath, Content);
						}

					}
				}

				var missingRes = parsedResources[0].SelectMany(x=> Regex.Matches(x.Content, @"(?i)(?<=url\(').+?(?='\))")).Cast<>()


				foreach (string file in Directory.GetFiles(textBox1.Text, "*.html").Select(File.ReadAllText))
				{

				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private (string Identifier, string TargetPath, string Content, string Comment) 
			ParseAzurePage(Match el, HashSet<string> takenIdentifiers, string OutputDirectory)
		{
			string Content = Regex.Match(el.Value, "(?i)(?<=<[^<>]*?>).*(?=<[^<>]*?>)").Value;
			string Identifier;
			string TargetPath;
			string Comment;

			var tag = Regex.Match(el.Value, "(?i)(?<=<)\\w+?(?=\\s+?)(?=[^<]*?>)").Value.ToLowerInvariant();

			string attr(string tagName, string attrName) =>
				Regex.Match(el.Value, $"(?i)(?<=<{tagName}\\s+?[^<>]*?{attrName}=\").*?(?=\"[^<]*?>)").Value;

			void setIdentifier(string path, string ext)
			{
				TargetPath = Regex.Replace(uniqaize(path) + ext, @"[^A-Za-z\.\\/]", "_");
				Identifier = Regex.Replace(TargetPath, "[^A-Za-z]", "_");
			}

			string uniqaize(string ident)
			{
				int i = 1;
				string result = ident;
				while (takenIdentifiers.Contains(result))
				{
					result = $"{ident}_{i}";
					i++;
				}
				takenIdentifiers.Add(result);
				return result;
			}


			switch (tag)
			{
				case "link":

					var href = attr("link", "href");

					var linkOverride = overrides.First(ov => ov.replaceHref == href);

					var linkOverrideFilePath = Path.Combine(OutputDirectory, linkOverride.sourcePath);
					if (File.Exists(linkOverrideFilePath))
					{
						Content = File.ReadAllText(linkOverrideFilePath);
					}
					else
					{
						var uri = new Uri(href, UriKind.RelativeOrAbsolute);
						if (!uri.IsAbsoluteUri)
							uri = currentUri.MakeRelativeUri(uri);

						Content = new HttpClient().GetStringAsync(uri).Result;
						File.WriteAllText(linkOverrideFilePath, Content);
					}

					Comment = attr("link", "href");
					setIdentifier(linkOverride.targetPath, linkOverride.extension);

					break;

				case "style":

					var extensionwindowid = attr("style", "data-extensionwindowid");
					var styleClass = attr("style", "class");

					if (!string.IsNullOrWhiteSpace(extensionwindowid))
					{
						Identifier = "ExtensionWindow/" + extensionwindowid;
						Comment = extensionwindowid;
					}
					else if (!string.IsNullOrWhiteSpace(styleClass))
					{
						Identifier = styleClass;
						Comment = styleClass;
					}
					else
					{
						Identifier = "UnnamedStylesheet";
						Comment = "Style hadn't any identifier";
					}

					Identifier = Regex.Replace(Identifier, "(?i)\\.css$", "");

					setIdentifier(Identifier, ".css");

					break;

				case "svg":

					Identifier = attr("symbol", "id");

					if (string.IsNullOrWhiteSpace(Identifier))
						throw new Exception("Svg symbol id not found");

					Comment = Identifier;

					setIdentifier("Svg/" + Identifier, ".svg");

					break;
				default:
					throw new Exception("Parse exception! Uknown element matched");
			}

			return (Identifier, TargetPath, Content, Comment);
		}
	}
}
