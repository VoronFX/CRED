using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebMarkupMin.AspNet.Common;
using WebMarkupMin.AspNetCore1;
using WebMarkupMin.Core;

namespace CRED
{
	public sealed class RuntimeResourcePacker
	{
		private readonly RuntimeResourcePackerOptions options;
		private readonly IHostingEnvironment hostingEnvironment;
		private readonly ICssMinifier cssMinifier;
		private readonly IMarkupMinifier markupMinifier;
		private readonly IJsMinifier jsMinifier;
		private readonly ILogger<RuntimeResourcePacker> logger;

		public RuntimeResourcePacker(IOptions<RuntimeResourcePackerOptions> options,
			IHostingEnvironment hostingEnvironment,
			IServiceProvider services)
		{
			this.options = options.Value;
			this.hostingEnvironment = hostingEnvironment;

			if (options.Value.EnableCssMinification)
				cssMinifier = services.GetService<ICssMinifierFactory>()?.CreateMinifier() ??
				              throw new InvalidOperationException($"No required service {nameof(ICssMinifierFactory)}");

			if (options.Value.EnableHtmlMinification)
				markupMinifier = services.GetService<IHtmlMinificationManager>()?.CreateMinifier() ??
				                 throw new InvalidOperationException($"No required service {nameof(HtmlMinificationManager)}");

			if (options.Value.EnableJsMinification)
				jsMinifier = services.GetService<IJsMinifierFactory>()?.CreateMinifier() ??
				             throw new InvalidOperationException($"No required service {nameof(IJsMinifierFactory)}");

			logger = services.GetService<ILoggerFactory>().CreateLogger<RuntimeResourcePacker>();

			PackDirectory(this.options.PacksDirectory).Wait();
		}

		private async Task PackDirectory(string directory)
		{
			await Task.WhenAll(hostingEnvironment
				.WebRootFileProvider
				.GetDirectoryContents(directory)
				.Where(x => !x.IsDirectory && x.Name.EndsWith(".js") && !x.Name.EndsWith(".pack.js"))
				.Select(x => PackFile(x, directory, options.WatchFilesForChanges, CancellationToken.None))
				.ToArray());
		}

		private async Task PackFile(IFileInfo file, string directory, bool watch, CancellationToken cancellationToken)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				var inputFile = await file.CreateReadStream().ReadFileToEndAsync();

				var tasks = Regex
					// Pre regex for perfrormace
					.Matches(inputFile, "(?s)new(?i).+?[\"\'].+?[\"\']", RegexOptions.Compiled)
					.Cast<Match>()
					.Select(match => match.Value)
					.SelectMany(x => Regex
						.Matches(x, $"(?s)(?<=new(?i)\\s+?{typeof(RequireResourceAttribute).FullName}.*?[\"']).+?(?=[\"'])",
							RegexOptions.Compiled)
						.Cast<Match>())
					.Select(match => match.Value)
					.Distinct()
					.Select(fileName =>
						Task.Run(async () =>
						{
							cancellationToken.ThrowIfCancellationRequested();

							var dependancyFile = await hostingEnvironment.WebRootFileProvider
								.GetFileInfo(fileName)
								.CreateReadStream()
								.ReadFileToEndAsync();

							dependancyFile = Minify(dependancyFile, fileName);

							return $"\"{fileName}\" : {JsonConvert.SerializeObject(dependancyFile)},{Environment.NewLine}";
						}, cancellationToken))
					.ToArray();

				await Task.WhenAll(tasks);

				var packedFile = $@"

// Resources embedded by {nameof(RuntimeResourcePacker)}
var {RequireResourceAttribute.ResourcesVariableName} = {{
{string.Join(string.Empty, tasks.Select(x => x.Result))}
}};

{inputFile}
";

				string minified;
				if (options.EnableJsMinification &&
				    TryMinify(jsMinifier.Minify(packedFile, false), file.PhysicalPath, out minified))
				{
					packedFile = minified;
				}

				File.WriteAllText(file.PhysicalPath.Replace(".js", ".pack.js"), packedFile);

				if (watch)
					RegisterWatch(file, directory, null);

				logger?.LogInformation($"File {file.PhysicalPath} packed", file);
			}
			catch (Exception e)
			{
				logger?.LogError($"Error while packing {file.PhysicalPath}", e);
				throw;
			}
		}

		private void RegisterWatch(IFileInfo file, string directory, string[] dependancies)
		{
			Tuple<Task, CancellationTokenSource> concurrentPacker = null;

			void Repack(object o)
			{
				var newToken = new CancellationTokenSource();
				var newPacker = new Task(async () => await PackFile(file, directory, true, newToken.Token), newToken.Token);

				var currentPacker = Interlocked.Exchange(ref concurrentPacker,
					new Tuple<Task, CancellationTokenSource>(newPacker, newToken));

				// ReSharper disable once MethodSupportsCancellation
				if (currentPacker == null)
					newPacker.Start();
				else
					currentPacker.Item1.ContinueWith(task => newPacker.Start());
			}

			hostingEnvironment.WebRootFileProvider
				.Watch(Path.Combine(directory, file.Name))
				.RegisterChangeCallback(Repack, null);

			//TODO: watch cahanges in dependacies

			//IDisposable[] watches = null;
			////Tuple<Task, CancellationTokenSource> concurrentPacker = null;

			//void Repack2(object o)
			//{

			//	var newToken = new CancellationTokenSource();
			//	var newPacker = new Task(async () => await PackFile(file, false, newToken.Token), newToken.Token);

			//	var currentPacker = Interlocked.Exchange(ref concurrentPacker,
			//		new Tuple<Task, CancellationTokenSource>(newPacker, newToken));

			//	// ReSharper disable once MethodSupportsCancellation
			//	currentPacker.Item1.ContinueWith(task => newPacker.Start());
			//}


			//watches = dependancies.Append(packFile).Select(x =>
			//	hostingEnvironment.WebRootFileProvider
			//		.Watch(x).RegisterChangeCallback(Repack, null))
			//		.ToArray();
		}

		private string Minify(string file, string fileName)
		{
			string minified;
			if ((options.EnableCssMinification && fileName.EndsWith(".css")
			     && TryMinify(cssMinifier.Minify(file, false), fileName, out minified))
			    || (options.EnableJsMinification && fileName.EndsWith(".js")
			        && TryMinify(jsMinifier.Minify(file, false), fileName, out minified))
			    || (options.EnableHtmlMinification && fileName.EndsWith(".html")
			        && TryMinify(markupMinifier.Minify(file), fileName, out minified)))
			{
				return minified;
			}
			return file;
		}

		private bool TryMinify<TMinificationResult>(TMinificationResult minificationResult, string fileName,
			out string minified)
			where TMinificationResult : MinificationResultBase
		{
			if (logger != null)
			{
				foreach (var error in minificationResult.Errors)
				{
					logger.LogError($"Error while minifiyng file {fileName} with message {error.Message}");
				}
				foreach (var warning in minificationResult.Warnings)
				{
					logger.LogError($"Warning while minifiyng file {fileName} with message {warning.Message}");
				}
			}
			minified = minificationResult.MinifiedContent;
			return !minificationResult.Errors.Any();
		}
	}

	public sealed class RuntimeResourcePackerOptions
	{
		public bool EnableCssMinification { get; set; }
		public bool EnableHtmlMinification { get; set; }
		public bool EnableJsMinification { get; set; }
		public bool WatchFilesForChanges { get; set; }
		public string PacksDirectory { get; set; }
	}

	internal static class RuntimeResourcePackerExtensionMethods
	{
		public static async Task<string> ReadFileToEndAsync(this Stream stream)
		{
			using (var inputStream = new StreamReader(stream))
			{
				return await inputStream.ReadToEndAsync();
			}
		}
	}
}
