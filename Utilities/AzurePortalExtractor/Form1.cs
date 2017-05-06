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
				new Extractor().ExtractAzureResources(textBox1.Text, textBox2.Text, textBox3.Text);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}