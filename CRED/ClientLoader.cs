using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CRED.Shared;
using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ResourceMapper.Base;

namespace CRED
{
	internal static class ClientLoaderResourcesExtensionMethods
	{
		private static string AppAssemblyName => typeof(Client.MainBundleFilesMap).Namespace;

		public static void AddClientLoader(this IServiceCollection services, IHostingEnvironment environment)
		{
			var staticSourcesDir = Path.GetFullPath($@"..\{AppAssemblyName}\Resources\Static");
			var generatedSourcesDir = Path.GetFullPath($@"..\{AppAssemblyName}\bin\Debug\net46\Generated\");

			if (environment.IsDevelopment()
				&& Directory.Exists(staticSourcesDir)
				&& Directory.Exists(generatedSourcesDir))
			{
				environment.WebRootFileProvider = new CompositeFileProvider(environment.WebRootFileProvider,
					new RequestPathFileProvider($"/{AppAssemblyName}/Static", new PhysicalFileProvider(staticSourcesDir)),
					new RequestPathFileProvider($"/{AppAssemblyName}/Generated", new PhysicalFileProvider(generatedSourcesDir)));

				services.AddSingleton(new ClientLoaderResources(
					GetFiles(new Client.PreloadFilesMap(null), true),
					GetFiles(new Client.MainStaticFilesMap(null), true)
					.Concat(GetFiles(new Client.MainGeneratedFilesMap(null), true)).ToArray()));
			}
			else
			{
				environment.WebRootFileProvider = new CompositeFileProvider(environment.WebRootFileProvider,
					new RequestPathFileProvider("/" + AppAssemblyName,
						new EmbeddedFileProvider(typeof(Client.MainBundleFilesMap).GetTypeInfo().Assembly)));

				services.AddSingleton(new ClientLoaderResources(
					GetFiles(new Client.PreloadBundleFilesMap(null), false),
					GetFiles(new Client.MainBundleFilesMap(null), false)));
			}

		}

		private static AppLoaderResource[] GetFiles(IResourceDirectory directory, bool fromSources)
			=> directory
				.GetFilesRecursive()
				.Select(file =>
				{
					var path = $"/{AppAssemblyName}" + string.Join(fromSources ? "/" : ".", 
						file.GetPath()); // + (fromSources ? $"?v={file.Hash}" : null);

					AppLoaderResource.ResourceType type;
					switch (Path.GetExtension(file.Name).ToLower())
					{
						case ".js":
							type = AppLoaderResource.ResourceType.Script;
							break;
						case ".css":
							type = AppLoaderResource.ResourceType.Style;
							break;
						case ".svg":
							type = AppLoaderResource.ResourceType.Svg;
							break;
						default:
							throw new NotSupportedException($"Unknown resource format {file.Name}");
					}
					return new AppLoaderResource(path, type);
				})
				.ToArray();
	}

	public class ClientLoaderResources
	{
		public AppLoaderResource[] PreloadFiles { get; }
		public AppLoaderResource[] MainFiles { get; }

		public ClientLoaderResources(AppLoaderResource[] preloadFiles, AppLoaderResource[] mainFiles)
		{
			PreloadFiles = preloadFiles;
			MainFiles = mainFiles;
		}
	}
}
