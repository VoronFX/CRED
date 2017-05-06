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

		private void ExtractFromAzureSourcesBtn_Click(object sender, EventArgs e)
		{
			try
			{
				//var _Content_Dynamic_UxFxStableCssCyrillic_C7498CEC16966F59B995A5ED9691B131213D24B7_css_c1_ru_c2_ru_ "df";
				//Viva_Controls__Controls__Visualization__Viva_Map_css = Viva_Controls__Controls__Visualization__Viva_Map_css;
				Directory.GetFiles(textBox1.Text, "*.html")
					.Select(File.ReadAllText)
					.Select(x =>
					{
						var head = Regex.Match(x, "(?i)(?<=<head>).*?(?=</head>)").Value;
						var takenIdentifiers = new HashSet<string>();

						Regex.Matches(head, "<(?:style|svg)[^<>]*?>.*?</(?:style|svg)>|<link[^<>]*?>")
							.Cast<Match>()
							.SelectMany(el =>
							{

								var Identifier = string.Empty;
								var Path = string.Empty;
								var Content = string.Empty;
								var Comment = string.Empty;

								//var content =>
								//var contentR = Regex.Match(el.Value, "(?i)(?<=<[^<>]*?>).*(?=<[^<>]*?>)").Value;
								//if (contentMatch s)
								var tag = Regex.Match(el.Value, "(?i)(?<=<)\\w+?(?=\\s+?)(?=[^<]*?>)").Value.ToLowerInvariant();

								string attr(string tagName, string attrName) =>
									Regex.Match(el.Value, $"(?i)(?<=<{tagName}\\s+?[^<>]*?{attrName}=\").*?(?=\"[^<]*?>)").Value;

								//string pathBase(string path) => Regex.Match(path, ".*?/").Value;
								//string toIdentifier(string text) => Regex.Replace(text, "[^A-Za-z]", "_");
								//string toPath(string text) => Regex.Replace(text, @"[^A-Za-z\.\\/]", "_");

								void setIdentifier(string path, string ext)
								{
									Path = Regex.Replace(uniqaize(path) + ext, @"[^A-Za-z\.\\/]", "_");
									Identifier = Regex.Replace(Path, "[^A-Za-z]", "_");
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

								string parseDependency(string text)
								{
									
								}
								//var TagMatcherPattern = "(?i)<(?<tag>\\w+?)(\\s|>)[^<>]*?"
								//                        + string.Join("", new[] {"type", "class", "href", "data-extensionwindowid", "rel", "property" }
								//						.Select(t => $"(?:{t}=\"(?<{t}>.*?)\")?[^<>]*?\n"));
								//"(?i)<link[^<>]*?type=\"text/css\"[^<>]*?href=\"(?<href>.*?)\"[^<>]*?>"

								Content = Regex.Match(el.Value, "(?i)(?<=<[^<>]*?>).*(?=<[^<>]*?>)").Value;
								var currentUri = new Uri("http://portal");
								switch (tag)
								{
									case "link":

										if (attr("link", "type") != "text/css")
											return;

										var href = attr("link", "href");

										var uri = new Uri(href, UriKind.RelativeOrAbsolute);
										if (!uri.IsAbsoluteUri)
											uri = currentUri.MakeRelativeUri(uri);
										currentUri = uri;

										Content = new HttpClient().GetStringAsync(currentUri).Result;



										Comment = attr("link", "href");
										setIdentifier("LinkHrefStylesheet", ".css");

										break;

									case "style":

										Identifier = attr("style", "class");

										if (string.IsNullOrWhiteSpace(Identifier))
											Identifier = "ExtensionWindow/" + attr("style", "data-extensionwindowid");

										if (string.IsNullOrWhiteSpace(Identifier))
											Identifier = "UnnamedStylesheet";

										Identifier = Regex.Replace(Identifier, "(?i)\\.css$", "");

										setIdentifier(Identifier, ".css");

										break;

									case "svg":

										Identifier = attr("symbol", "id");
										if (string.IsNullOrWhiteSpace(Identifier))
											return;

										setIdentifier("Svg/" + Identifier, ".svg");

										break;
									default:
										throw new Exception("Parse exception! Uknown element matched");
								}



								//return new
								//{
								//	Identifier = "s",
								//	Path = "fas",
								//	Content = "sda"
								//	Comment = "sda"
								//}
							});


						return new
						{
							Styles = Regex.Matches(
						Regex.Match(x, "(?i)(?<=<div\\s+?class(=[\"'][\"']>)?).*?(?=</div>)").Value,
						"(?<=<style\\s+?class=[\"\'](?<name>[^\"\']+?)[\"\']>).*?(?=</style>)").Cast<Match>()
						.Select(x => new { Name = x.Groups["name"], x.Value }),

							Symbols = Regex.Match()
						}
					})

				foreach (string file in Directory.GetFiles(textBox1.Text, "*.html").Select(File.ReadAllText))
				{

				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
