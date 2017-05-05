using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CRED
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddRuntimeResourcePacker(this IServiceCollection services,
			Action<RuntimeResourcePackerOptions> configure)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (configure == null)
			{
				throw new ArgumentNullException(nameof(configure));
			}

			services.Configure(configure);
			services.AddSingleton(typeof(RuntimeResourcePacker));

			return services;
		}

		public static IApplicationBuilder UseRuntimeResourcePacker(this IApplicationBuilder app)
		{
			app.ApplicationServices.GetRequiredService<RuntimeResourcePacker>();

			return app;
		}
	}
}