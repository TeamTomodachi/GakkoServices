using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using GakkoServices.AuthServer.Data.Contexts;
using GakkoServices.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Hosting;
using RawRabbit;
using RawRabbit.vNext;
using GakkoServices.AuthServer.Business.Services;
using GakkoServices.Core.Services;
using GakkoServices.Core.Helpers;
using System.Net;
using IdentityServer4.Stores;
using IdentityServer4.EntityFramework.Stores;
using GakkoServices.AuthServer.BackgroundServices;

namespace GakkoServices.AuthServer
{
    public class Startup
    {
        public const string SERVICE_ENDPOINT_REWRITE = "auth";
        public const string CORS_POLICY = "default";

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = environment;
            _logger = logger;

            // Setup Configuraiton
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("secretappsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secretappsettings.{Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AuthServerDatabaseConfiguration databaseConfig = new AuthServerDatabaseConfiguration(Configuration, null);

            // Configure Application Users
            services.AddDbContext<AspIdentityDbContext>(options => databaseConfig.BuildDBContext(options));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AspIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Add Cors
            // http://docs.identityserver.io/en/latest/quickstarts/6_javascript_client.html
            services.AddCors(options =>
            {
                options.AddPolicy(Startup.CORS_POLICY, policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddCors();

            // Add MVC
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Auth Server API", Version = "v1" });
            });

            //// Configure IdentityServer
            // configure identity server with in-memory stores, keys, clients and scopes

            var builder = services.AddIdentityServer(options =>
            {
                options.Discovery.ShowTokenEndpointAuthenticationMethods = true;
                options.Discovery.CustomEntries.Add("UserAccount", "~/api/UserAccount");
            }).AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(options => // this adds the config data from DB (clients, resources)
            {
                options.ConfigureDbContext = b => databaseConfig.BuildDBContext(b);
            })
            .AddOperationalStore(options => // this adds the operational data from DB (codes, tokens, consents)
            {
                options.ConfigureDbContext = b => databaseConfig.BuildDBContext(b);
                
                options.EnableTokenCleanup = true; // this enables automatic token cleanup. this is optional.
            })
            .AddDeveloperSigningCredential();

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            // ToDo: Change the IdentityServer Endpoints
            // https://stackoverflow.com/questions/39186533/change-default-endpoint-in-identityserver-4
            //builder.Services
            // .Where(service => service.ServiceType == typeof(Endpoint))
            // .Select(item => (Endpoint)item.ImplementationInstance)
            // .ToList()
            // .ForEach(item =>
            //     {
            //         if (item.Path.Value.Contains("/connect"))
            //         {
            //             item.Path = item.Path.Value.Replace("/connect", $"/{AUTHSERVER_ENDPOINT_REWRITE}/connect");
            //         }
            //         else if (item.Path.Value.Contains("/.well-known"))
            //         {
            //             item.Path = item.Path.Value.Replace("/.well-known", $"/{AUTHSERVER_ENDPOINT_REWRITE}/.well-known");
            //         }
            //     });

            // Add in Authentication Providers
            // services.AddAuthentication(IdentityServerConstants.DefaultCookieAuthenticationScheme)
            //     .AddCookie(IdentityServerConstants.DefaultCookieAuthenticationScheme, options =>
            //     {
            //         options.Cookie.Path = "/";
            //         options.LoginPath = "/auth/account/login";
            //         options.LogoutPath = "/auth/account/logout";
            //     })

            //// Create the Cookie Builder
            //CookieBuilder cookieBuilder = new CookieBuilder();
            //cookieBuilder.Domain = ".pogogakko.com";
            //cookieBuilder.Path = "/";

            // Add Authentication Providers
            services.AddAuthentication()
                // .AddCookie(options =>
                // {
                    // options.Cookie = cookieBuilder;
                // })
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = Configuration["ExternalAuthenticationProviders:Google:ClientId"];
                    options.ClientSecret = Configuration["ExternalAuthenticationProviders:Google:ClientSecret"];
                })
                .AddDiscord("Discord", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = Configuration["ExternalAuthenticationProviders:Discord:ClientId"];
                    options.ClientSecret = Configuration["ExternalAuthenticationProviders:Discord:ClientSecret"];
                });
            //.AddJwtBearer(options => // For debugging
            //{
            //    options.Authority = Configuration["Auth0:Authority"];
            //    options.Audience = Configuration["Auth0:Audience"];
            //    options.RequireHttpsMetadata = false;
            //});

            var keysDir = Path.Combine(Directory.GetCurrentDirectory(), "/keys");
            _logger.LogInformation("Persisting dataprotection keys to {}", keysDir);
            services.AddDataProtection()
                .SetApplicationName("authserver")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(keysDir));

            // Configure RabbitMq
            services.AddRawRabbit(options =>
            {
                options.SetBasePath(Environment.ContentRootPath)
                    .AddJsonFile("rawrabbit.json")
                    .AddEnvironmentVariables("RawRabbit:");
            });

            _logger.LogInformation("Waiting for rabbitmq...");
            // Block until the rabbitmq panel is online
            NetworkingHelpers.WaitForOk(new Uri("http://rabbitmq:15672")).Wait();
            _logger.LogInformation("rabbitmq is ready");

            if (Environment.IsDevelopment())
            {
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            // Configure Dependencies
            services.AddScoped<AccountService, AccountService>();

            // Additional Configuration
            services.AddHttpContextAccessor();
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, MessageHandlerService>();
            services.AddSingleton<ContextServiceLocator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Change the Root Path of the AuthServer
            app.UsePathBase($"/{SERVICE_ENDPOINT_REWRITE}");

            // Initialize our Databases
            try
            {
                AuthServerDatabaseConfiguration databaseConfig = new AuthServerDatabaseConfiguration(Configuration, app);
                databaseConfig.InitializeDatabase(app);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Caught exception when initializing DB: {Exception}", e);
            }

            // Configure our Error Pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{SERVICE_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "Auth Server API");
            });

            // Enable CORS
            app.UseCors(Startup.CORS_POLICY);
            app.UseCors(
                options => options.AllowAnyOrigin()//.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            // Setup our pipeline to use Static Files...
            app.UseStaticFiles();

            // Load in IdentityServer Middleware
            app.UseIdentityServer();

            // Setup MVC with a Default Route
            app.UseMvcWithDefaultRoute();
            //app.UseMvc();
        }
    }
}
