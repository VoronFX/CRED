using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CRED.Data;
using CRED.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NSwag;
using NSwag.SwaggerGeneration.WebApi.Processors.Security;

namespace CRED2
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			Env = env;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Env { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddDbContext<CREDContext>(options =>
			{
				var efOptions = options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

				if (Env.IsDevelopment())
					efOptions.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));

				// Register the entity sets needed by OpenIddict.
				// Note: use the generic overload if you need
				// to replace the default OpenIddict entities.
				//options.UseOpenIddict();
			});

			// Register the Identity services.
			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<CREDContext>()
				.AddDefaultTokenProviders();

			// Configure Identity to use the same JWT claims as OpenIddict instead
			// of the legacy WS-Federation claims it uses by default (ClaimTypes),
			// which saves you from doing the mapping in your authorization controller.
			//services.Configure<IdentityOptions>(options =>
			//{
			//	options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
			//	options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
			//	//options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
			//});

			// Register the OpenIddict services.
			//services.AddOpenIddict(options =>
			//{
			//	// Register the Entity Framework stores.
			//	options.AddEntityFrameworkCoreStores<CREDContext>();

			//	// Register the ASP.NET Core MVC binder used by OpenIddict.
			//	// Note: if you don't call this method, you won't be able to
			//	// bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
			//	options.AddMvcBinders();

			//	// Enable the authorization, logout, token and userinfo endpoints.
			//	options.EnableAuthorizationEndpoint("/connect/authorize")
			//		   .EnableLogoutEndpoint("/connect/logout")
			//		   .EnableTokenEndpoint("/connect/token")
			//		   .EnableUserinfoEndpoint("/api/userinfo");

			//	// Note: the Mvc.Client sample only uses the authorization code flow but you can enable
			//	// the other flows if you need to support implicit, password or client credentials.
			//	options.AllowPasswordFlow();

			//	// When request caching is enabled, authorization and logout requests
			//	// are stored in the distributed cache by OpenIddict and the user agent
			//	// is redirected to the same page with a single parameter (request_id).
			//	// This allows flowing large OpenID Connect requests even when using
			//	// an external authentication provider like Google, Facebook or Twitter.
			//	// options.EnableRequestCaching();

			//	// During development, you can disable the HTTPS requirement.
			//	//if (CurrentEnvironment.IsDevelopment())
			//	{
			//		options.DisableHttpsRequirement();
			//	}
			//});

			services.Add(ServiceDescriptor.Singleton(typeof(GitRepositoryService), typeof(GitRepositoryService)));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, CREDContext dbcontext, GitRepositoryService gitRepositoryService)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (Env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			if (Env.IsDevelopment())
			{
				app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
				{
					HotModuleReplacement = true,
				});
			}

			// Add a middleware used to validate access
			// tokens and protect the API endpoints.
			//app.UseOAuthValidation();

			// Alternatively, you can also use the introspection middleware.
			// Using it is recommended if your resource server is in a
			// different application/separated from the authorization server.
			//
			// app.UseOAuthIntrospection(options =>
			// {
			//     options.AutomaticAuthenticate = true;
			//     options.AutomaticChallenge = true;
			//     options.Authority = "http://localhost:58795/";
			//     options.Audiences.Add("resource_server");
			//     options.ClientId = "resource_server";
			//     options.ClientSecret = "875sqd4s5d748z78z7ds1ff8zz8814ff88ed8ea4z4zzd";
			// });

			//app.UseOpenIddict();

			//app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiOwinSettings()
			//{
			//	OperationProcessors =
			//	{
			//		new OperationSecurityScopeProcessor("apikey")
			//	},
			//	DocumentProcessors =
			//	{
			//		new SecurityDefinitionAppender("apikey", new SwaggerSecurityScheme
			//		{
			//			Type = SwaggerSecuritySchemeType.ApiKey,
			//			Name = "Authorization",
			//			In = SwaggerSecurityApiKeyLocation.Header
			//		})
			//	},
			//	DefaultPropertyNameHandling = PropertyNameHandling.CamelCase
			//});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new CompositeFileProvider(new PhysicalFileProvider(Path.GetFullPath("node_modules/")), Env.WebRootFileProvider),
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

			// if you want to use automated deployments, keep the following line remarked out
			// if (CurrentEnvironment.IsDevelopment())
			{
				DbInitializer.Initialize(dbcontext);
			}

		}
	}
}
