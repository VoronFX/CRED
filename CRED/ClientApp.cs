using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CRED.Client.Pack;
using Dazinator.AspNet.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ResourceMapper.Base;

namespace CRED
{
	public static class ClientApp
	{
		public static string AppAssemblyName { get; } = "CRED.Client";

		public static ClientPack Resources { get; } = new ClientPack(null);

		public static string GetClientAppDirUrl(this IResourceDirectory resource, bool fromSources = false)
			=> $"/{AppAssemblyName}/" + string.Join(fromSources ? "/" : ".", resource.GetPath());

		public static string GetClientAppFileUrl(this IResourceFile resource, bool fromSources = false, bool addVersionHash = false)
			=> resource.ContainingDirectory.GetClientAppDirUrl(fromSources)
				+ (fromSources ? "/" : ".")
				+ resource.Name
				+ (addVersionHash ? $"?v={resource.Hash}" : null);

		public static void AddClientApp(this IServiceCollection services, IHostingEnvironment environment)
		{
			var sourcesDir = Path.GetFullPath($@"..\{AppAssemblyName}\bin\Debug\net46\wwwroot\");

			if (environment.IsDevelopment()
				&& Directory.Exists(sourcesDir))
			{
				environment.WebRootFileProvider = new CompositeFileProvider(environment.WebRootFileProvider,
					new RequestPathFileProvider($"/{AppAssemblyName}", new PhysicalFileProvider(sourcesDir)));
			}
			else
			{
				environment.WebRootFileProvider = new CompositeFileProvider(environment.WebRootFileProvider,
					new RequestPathFileProvider("/" + AppAssemblyName,
						new EmbeddedFileProvider(typeof(ClientPack).GetTypeInfo().Assembly)));
			}

		}
	}

}
