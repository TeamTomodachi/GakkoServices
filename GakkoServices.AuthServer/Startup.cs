using System;
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

namespace GakkoServices.AuthServer
{
    public class Startup
    {
        const string AUTHSERVER_ENDPOINT_REWRITE = "authserver";
        const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True";

        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Connection String

            // Configure Application Users
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
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
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
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

                    options.ClientId = "<insert here>";
                    options.ClientSecret = "<inser here>";
                })
                // Not sure if should be in AuthServer...
                .AddOpenIdConnect("oidc", "IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.Authority = "http://localhost:5001";
                    options.ClientId = "implicit";
                    options.ResponseType = "id_token";
                    options.SaveTokens = true;
                    options.CallbackPath = new PathString("/signin-idsrv");
                    options.SignedOutCallbackPath = new PathString("/signout-callback-idsrv");
                    options.RemoteSignOutPath = new PathString("/signout-idsrv");
                    options.RequireHttpsMetadata = false; // For debugging

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
            //.AddJwtBearer(options => // For debugging
            //{ 
            //    options.Authority = Configuration["Auth0:Authority"];
            //    options.Audience = Configuration["Auth0:Audience"];
            //    options.RequireHttpsMetadata = false; 
            //}); 

            if (Environment.IsDevelopment())
            {
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Change the Root Path of the AuthServer
            app.UsePathBase($"/{AUTHSERVER_ENDPOINT_REWRITE}");

            // Initialize our Databases
            //InitializeDatabase(app);

            // Configure our Error Pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Setup http to https redirection
            app.UseHttpsRedirection();

            // Setup our pipeline to use Static Files...
            app.UseStaticFiles();

            // Load in IdentityServer Middleware
            app.UseIdentityServer();

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{AUTHSERVER_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "Auth Server API");
            });

            // Setup MVC with a Default Route
            app.UseMvcWithDefaultRoute();
            //app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // Migrate the ApplicationDbContext
                var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationDbContext.Database.Migrate();

                // Migrate the Persisted Grant DB Context
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                // Migrate the ConfigurationDbContext
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
