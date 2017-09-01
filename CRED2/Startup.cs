using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace CRED2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

	        if (env.IsDevelopment())
	        {
		        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
		        {
			        HotModuleReplacement = true,
		        });
	        }

	        app.UseStaticFiles(new StaticFileOptions
	        {
		        FileProvider = new CompositeFileProvider(new PhysicalFileProvider(Path.GetFullPath("node_modules/")), env.WebRootFileProvider),
		        OnPrepareResponse = context =>
		        {
			        // Cache static file for 1 year
			        if (!string.IsNullOrEmpty(context.Context.Request.Query["v"]))
			        {
				        context.Context.Response.Headers.Add("cache-control", new[] { "public,max-age=31536000" });
				        context.Context.Response.Headers.Add("Expires",
					        new[] { DateTime.UtcNow.AddYears(1).ToString("R") }); // Format RFC1123
			        }
		        },
	        });

			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
