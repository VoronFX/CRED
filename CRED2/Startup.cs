using System;
using System.IO;
using System.Linq;
using System.Reflection;

using AspNet.Security.OpenIdConnect.Primitives;

using CRED.Data;

using CRED2.Data;
using CRED2.Helpers;
using CRED2.Model;
using CRED2.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using Newtonsoft.Json;

namespace CRED2
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.Configuration = configuration;
            this.Env = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, CREDContext dbcontext, GitBridgeService gitBridgeService)
        {
            if (this.Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();

            if (this.Env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions { HotModuleReplacement = true, });
            }

            // Add a middleware used to validate access
            // tokens and protect the API endpoints.
            // app.UseOAuthValidation();

            // Alternatively, you can also use the introspection middleware.
            // Using it is recommended if your resource server is in a
            // different application/separated from the authorization server.
            // app.UseOAuthIntrospection(options =>
            // {
            // options.AutomaticAuthenticate = true;
            // options.AutomaticChallenge = true;
            // options.Authority = "http://localhost:58795/";
            // options.Audiences.Add("resource_server");
            // options.ClientId = "resource_server";
            // options.ClientSecret = "875sqd4s5d748z78z7ds1ff8zz8814ff88ed8ea4z4zzd";
            // });

            // app.UseOpenIddict();

            // app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiOwinSettings()
            // {
            // OperationProcessors =
            // {
            // new OperationSecurityScopeProcessor("apikey")
            // },
            // DocumentProcessors =
            // {
            // new SecurityDefinitionAppender("apikey", new SwaggerSecurityScheme
            // {
            // Type = SwaggerSecuritySchemeType.ApiKey,
            // Name = "Authorization",
            // In = SwaggerSecurityApiKeyLocation.Header
            // })
            // },
            // DefaultPropertyNameHandling = PropertyNameHandling.CamelCase
            // });

            app.UseStaticFiles(
                new StaticFileOptions
                    {
                        FileProvider = new CompositeFileProvider(
                            new PhysicalFileProvider(Path.GetFullPath("node_modules/")),
                            this.Env.WebRootFileProvider),
                        OnPrepareResponse = context =>
                            {
                                // Cache static file for 1 year
                                if (!string.IsNullOrEmpty(context.Context.Request.Query["v"]))
                                {
                                    context.Context.Response.Headers.Add(
                                        "cache-control",
                                        new[] { "public,max-age=31536000" });
                                    context.Context.Response.Headers.Add(
                                        "Expires",
                                        new[]
                                            {
                                                DateTime.UtcNow.AddYears(1).ToString("R")
                                            }); // Format RFC1123
                                }
                            },
                    });

            app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{controller}/{action=Index}/{id?}"); });

            // if you want to use automated deployments, keep the following line remarked out
            {
                // if (CurrentEnvironment.IsDevelopment())
                DbInitializer.Initialize(dbcontext);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(
                options =>
                    {
                        var settings = options.SerializerSettings;
                        settings.TypeNameHandling = TypeNameHandling.Auto;
                        settings.SerializationBinder = new KnownTypesBinder
                                                           {
                                                               KnownTypes =
                                                                   TaskRequestService
                                                                       .DiscoverRunnerTypes(
                                                                           Assembly
                                                                               .GetExecutingAssembly())
                                                                       .SelectMany(
                                                                           TaskRequestService
                                                                               .DiscoverImplementedRunnerInterfaces)
                                                                       .Select(
                                                                           x => TaskRequestService
                                                                               .RunnerInterfaceRequestDataType(
                                                                                   x.GetTypeInfo()))
                                                                       .ToArray()
                                                           };
                    });
            services.AddMemoryCache();

            var efConfigureOptions = new Action<DbContextOptionsBuilder>(
                options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                        if (Env.IsDevelopment())
                            options.ConfigureWarnings(
                                warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));

                        // Register the entity sets needed by OpenIddict.
                        // Note: use the generic overload if you need
                        // to replace the defaul1t OpenIddict entities.
                        options.UseOpenIddict();
                    });

            var efOptionsBuilder = new DbContextOptionsBuilder<CREDContext>();
            efConfigureOptions(efOptionsBuilder);
            var efOptions = efOptionsBuilder.Options;

            services.AddDbContext<CREDContext>(efConfigureOptions);
            services.AddTransient<Func<CREDContext>>(provider => () => new CREDContext(efOptions));

            // services.AddTransient<IDesignTimeDbContextFactory<CREDContext>>(provider => new CREDContext(efOptions));

            // Register the Identity services.
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<CREDContext>()
                .AddDefaultTokenProviders();

            // Register the OAuth2 validation handler.
            services.AddAuthentication().AddOpenIdConnect(
                options =>
                    {
                        options.ClientId = Configuration["ClientId"];
                        options.ClientSecret = Configuration["ClientSecret"];
                        options.Authority = Configuration["Authority"];
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.GetClaimsFromUserInfoEndpoint = true;
                    }).AddOAuthValidation();

            // app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            // {
            // ClientId = Configuration["ClientId"],
            // ClientSecret = Configuration["ClientSecret"],
            // Authority = Configuration["Authority"],
            // ResponseType = OpenIdConnectResponseType.Code,
            // GetClaimsFromUserInfoEndpoint = true
            // });
            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(
                options =>
                    {
                        options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                        options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                        options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                    });

            // Register the OpenIddict services.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            services.AddOpenIddict(
                options =>
                    {
                        // Register the Entity Framework stores.
                        options.AddEntityFrameworkCoreStores<CREDContext>();

                        // Register the ASP.NET Core MVC binder used by OpenIddict.
                        // Note: if you don't call this method, you won't be able to
                        // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                        options.AddMvcBinders();

                        // Enable the authorization, logout, token and userinfo endpoints.
                        options.EnableAuthorizationEndpoint("/connect/authorize").EnableTokenEndpoint("/connect/token");

                        // Allow client applications to use the grant_type=password flow.
                        options.AllowPasswordFlow();
                        options.AllowAuthorizationCodeFlow();

                        // When request caching is enabled, authorization and logout requests
                        // are stored in the distributed cache by OpenIddict and the user agent
                        // is redirected to the same page with a single parameter (request_id).
                        // This allows flowing large OpenID Connect requests even when using
                        // an external authentication provider like Google, Facebook or Twitter.
                        options.EnableRequestCaching();

                        // During development, you can disable the HTTPS requirement.
                        if (Env.IsDevelopment())
                        {
                            options.DisableHttpsRequirement();
                        }
                    });

            foreach (var runnerType in TaskRequestService.DiscoverRunnerTypes(Assembly.GetExecutingAssembly()))
            {
                foreach (var runnerInt in TaskRequestService.DiscoverImplementedRunnerInterfaces(runnerType))
                {
                    services.Add(ServiceDescriptor.Transient(runnerInt, runnerType));
                }
            }
            services.Add(ServiceDescriptor.Singleton(typeof(TaskRequestService), typeof(TaskRequestService)));
            services.Add(ServiceDescriptor.Singleton(typeof(TaskRequestService), typeof(TaskRequestService)));
            services.Add(ServiceDescriptor.Singleton(typeof(HistoryRepository), typeof(HistoryRepository)));
            services.Add(ServiceDescriptor.Singleton(typeof(GitBridgeService), typeof(GitBridgeService)));
        }
    }
}