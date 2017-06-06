using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ResourceMapper;
using WebMarkupMin.Core;
using CsCodeGenerator;

namespace BundlerAndMinifier
{
	public partial class BundlerAndMinifier
	{
		private BundlerAndMinifierTask Task { get; }

		private Lazy<ICssMinifier> CssMinifier { get; }
		private Lazy<IJsMinifier> JsMinifier { get; }
		private Lazy<IMarkupMinifier> MarkupMinifier { get; }
		private Lazy<IMarkupMinifier> XmlMinifier { get; }

		static BundlerAndMinifier()
		{
			Execute = (task) => new BundlerAndMinifier(task).BundlerAndMinify();
		}

		private BundlerAndMinifier(BundlerAndMinifierTask task)
		{
			Task = task;
			CssMinifier = new Lazy<ICssMinifier>(() =>
				new KristensenCssMinifier(), LazyThreadSafetyMode.PublicationOnly);
			JsMinifier = new Lazy<IJsMinifier>(() =>
				new CrockfordJsMinifier(), LazyThreadSafetyMode.PublicationOnly);
			XmlMinifier = new Lazy<IMarkupMinifier>(() =>
				new XmlMinifier(), LazyThreadSafetyMode.PublicationOnly);
			MarkupMinifier = new Lazy<IMarkupMinifier>(() =>
				new HtmlMinifier(null, CssMinifier.Value, JsMinifier.Value), LazyThreadSafetyMode.PublicationOnly);

			Task.InputFiles = Task.InputFiles ?? Array.Empty<string>();
		}

		private void BundlerAndMinify()
		{
			var combined = Task.InputFiles
				.AsParallel()
				.AsOrdered()
				.Select(file => Minify(File.ReadAllText(file), file))
				.Aggregate(new StringBuilder(), (builder, s) => builder.AppendLine(s));

			IOExtension.EnsureFileDirectoryCreated(Task.OutputFile);
			File.WriteAllText(Task.OutputFile, Minify(combined.ToString(), Task.OutputFile));
		}

		private string Minify(string content, string filename)
		{
			var ext = Path.GetExtension(Task.OutputFile);
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
				Log(LogCategory.Warning, Format(warning));
			}

			return result.MinifiedContent;
		}
	}
}