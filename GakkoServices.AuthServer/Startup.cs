﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GakkoServices.AuthServer
{
    public class Startup
    {
        const string SERVICE_ENDPOINT_REWRITE = "auth";

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

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

            // Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Auth Server API", Version = "v1" });
            });

            //// Configure IdentityServer
            // configure identity server with in-memory stores, keys, clients and scopes

            var builder = services.AddIdentityServer(options =>
            {
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
            services.AddAuthentication()
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

            services.AddRawRabbit(options =>
            {
                options.SetBasePath(Environment.ContentRootPath)
                    .AddJsonFile("rawrabbit.json")
                    .AddEnvironmentVariables("RawRabbit:");
            });

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
            catch (Exception) { }

            // Configure our Error Pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Setup our pipeline to use Static Files...
            app.UseStaticFiles();

            // Load in IdentityServer Middleware
            app.UseIdentityServer();

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{SERVICE_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "Auth Server API");
            });

            // Setup MVC with a Default Route
            app.UseMvcWithDefaultRoute();
            //app.UseMvc();
        }
    }
}
