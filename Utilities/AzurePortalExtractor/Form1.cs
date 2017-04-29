using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
	}
}
