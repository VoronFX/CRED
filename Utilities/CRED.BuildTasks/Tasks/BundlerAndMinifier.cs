using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMarkupMin.Core;
using CsCodeGenerator;
using Microsoft.Build.Framework;

namespace CRED.BuildTasks
{
	public sealed class BundlerAndMinifier : TaskBase
	{
		[Required]
		[ExpandPath]
		[DataMember]
		public string[] InputFiles { get; set; }

		[Required]
		[ExpandPath]
		[EnsureDirectoryCreated]
		[DataMember]
		public string OutputFile { get; set; }

		private Lazy<ICssMinifier> CssMinifier { get; }
		private Lazy<IJsMinifier> JsMinifier { get; }
		private Lazy<IMarkupMinifier> MarkupMinifier { get; }
		private Lazy<IMarkupMinifier> XmlMinifier { get; }

		public BundlerAndMinifier()
		{
			CssMinifier = new Lazy<ICssMinifier>(() =>
				new KristensenCssMinifier(), LazyThreadSafetyMode.PublicationOnly);
			JsMinifier = new Lazy<IJsMinifier>(() =>
				new CrockfordJsMinifier(), LazyThreadSafetyMode.PublicationOnly);
			XmlMinifier = new Lazy<IMarkupMinifier>(() =>
				new XmlMinifier(), LazyThreadSafetyMode.PublicationOnly);
			MarkupMinifier = new Lazy<IMarkupMinifier>(() =>
				new HtmlMinifier(null, CssMinifier.Value, JsMinifier.Value), LazyThreadSafetyMode.PublicationOnly);
		}

		protected override bool ExecuteWork()
		{
			InputFiles = InputFiles ?? Array.Empty<string>();

			BuildIncrementally(InputFiles, inputFiles =>
			{
				var combined = InputFiles
					.AsParallel()
					.AsOrdered()
					.Select(file => Minify(File.ReadAllText(file), file))
					.Aggregate(new StringBuilder(), (builder, s) => builder.AppendLine(s));

				File.WriteAllText(OutputFile, Minify(combined.ToString(), OutputFile));

				return new[] { OutputFile };
			});

			return true;
		}

		private string Minify(string content, string filename)
		{
			var ext = Path.GetExtension(OutputFile);
			bool EnxtensionMatch(params string[] extensions)
				=> extensions.Any(x => string.Equals(ext, x, StringComparison.OrdinalIgnoreCase));

			MinificationResultBase result;
			if (EnxtensionMatch(".css"))
			{
				result = CssMinifier.Value.Minify(content, false);
			}
			else if (EnxtensionMatch(".html"))
			{
				result = MarkupMinifier.Value.Minify(content, false);
			}
			else if (EnxtensionMatch(".svg", ".xml"))
			{
				result = XmlMinifier.Value.Minify(content, false);
			}
			else if (EnxtensionMatch(".js"))
			{
				result = JsMinifier.Value.Minify(content, false);
			}
			else
			{
				throw new NotSupportedException($"Unknown file format {ext}");
			}

			string Format(MinificationErrorInfo info)
				=> string.Join(Environment.NewLine,
					$@"{info.Category}: {info.Message}",
					$@"Line:{info.LineNumber} Col:{info.ColumnNumber}",
					$@"File:{filename}",
					$@"Source:{info.SourceFragment}");

			foreach (var error in result.Errors)
			{
				throw new Exception(Format(error));
			}

			foreach (var warning in result.Warnings)
			{
				Log.LogWarning(Format(warning));
			}

			return result.MinifiedContent;
		}
	}
}