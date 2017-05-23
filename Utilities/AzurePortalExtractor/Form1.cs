using System;
using System.Collections;
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
using System.Xml;
using HtmlAgilityPack;
using FastColoredTextBoxNS;

// ReSharper disable InconsistentNaming

namespace AzurePortalExtractor
{
	public sealed partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			Input = new FastColoredTextBox()
			{
				Parent = this,
				Anchor = InputOld.Anchor,
				Left = InputOld.Left,
				Top = InputOld.Top,
				Width = InputOld.Width,
				Height = InputOld.Height,
				Language = Language.HTML,
				BackColor = BackColor,
				AutoIndent = true
			};
			Input.TextChanged += (sender, args) =>
			{
				Input.Language = (Input.Lines.Count > 0 && Input.Lines[0].Contains("<")) ? Language.HTML : Language.CSharp;
			};
			ReactBindingsText = InputOld.Text;
			Controls.Remove(InputOld);
		}

		public string ReactBindingsText { get; }
		public FastColoredTextBox Input { get; }

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
				if (!string.IsNullOrWhiteSpace(Input.Text))
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
				if (!string.IsNullOrWhiteSpace(Input.Text))
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
				new Extractor().ExtractAzureResources(textBox1.Text, textBox2.Text, textBox3.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(Input.Text))
					Input.Text = Clipboard.GetText();

				Input.Text = string.Join(Environment.NewLine,
					HtmlToReactConverter.CreateElement(HtmlNode.CreateNode(Input.Text)).Where(x =>
						!Regex.IsMatch(x, @"\s*//\s\$1")));
				Input.Text = Regex.Replace(Input.Text, "(?si)(?<=[})]),(?=\\s+[})])", "");

				Input.SelectAll();
				if (!string.IsNullOrWhiteSpace(Input.Text))
					Clipboard.SetText(Input.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Input.Text = String.Empty;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			Input.SelectAll();
			if (!string.IsNullOrWhiteSpace(Input.Text))
				Clipboard.SetText(Input.Text);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var t = Clipboard.GetText();
			Input.SelectAll();
			if (!string.IsNullOrWhiteSpace(Input.Text))
				Clipboard.SetText(Input.Text);
			Input.Text = t;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			string name = "DomBuilder";

			var s = string.Join(Environment.NewLine, Regex.Matches(ReactBindingsText, "(?si)(?<=extern ReactElement )(?<el>\\w+)\\((?<prop>\\w+)(?=\\s+properties)")
				.Cast<Match>()
				.Select(x => x.Groups["el"].Value + ",")
				.Distinct());

			File.WriteAllLines(textBox4.Text,
			DefinitionGenerator.GenerateNamespace(
				name: "FluentReact",
				comment: new[] { "Content generated by React Bindings Maker" },
				imports: new [] { "System", "Bridge.React" },
				content: DefinitionGenerator.GenerateClass(
					name: name,
					comment: new string[] { },
					@static: false,
					partial: true,
					@public: true,
					@sealed: true,
					content: Regex.Matches(ReactBindingsText, "(?si)(?<=extern ReactElement )(?<el>\\w+)\\((?<prop>\\w+)(?=\\s+properties)")
						.Cast<Match>()
						.Select(x => new { el = x.Groups["el"].Value, prop = x.Groups["prop"].Value })
						.Distinct().SelectMany(x => new[]
							{
								$@"public {name} {x.el}(Action<{x.prop}> propSetter, {name} children = null)",
								"    => AddElements(propSetter, children);",
								string.Empty, 
							}
						)
					)));

			Input.Text = File.ReadAllText(textBox4.Text);

		}
	}
}