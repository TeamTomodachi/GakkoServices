using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.Microservices.ProfileService.Data.Contexts;
using GakkoServices.Microservices.ProfileService.BackgroundServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Hosting = Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GakkoServices.Core.Messages;
using GakkoServices.Core.Helpers;
using RawRabbit;
using RawRabbit.vNext;

namespace GakkoServices.Microservices.ProfileService
{
    public class Startup
    {
        const string SERVICE_ENDPOINT_REWRITE = "profile";

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
            ProfileServiceDatabaseConfiguration databaseConfig = new ProfileServiceDatabaseConfiguration(Configuration, null);

            // Configure DBContexts
            services.AddDbContext<ProfileServiceDbContext>(options => databaseConfig.BuildDBContext(options));

            // Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddRawRabbit(options =>
            {
                options.SetBasePath(Environment.ContentRootPath)
                    .AddJsonFile("rawrabbit.json")
                    .AddEnvironmentVariables("RawRabbit:");
            });

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Profile Service API", Version = "v1" });
            });

            services.AddSingleton<Hosting.IHostedService, ProfileMessageHandlerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger logger)
        {
            // Change the Root Path of the Profile
            app.UsePathBase($"/{SERVICE_ENDPOINT_REWRITE}");

            // Initialize our Databases
            try
            {
                ProfileServiceDatabaseConfiguration databaseConfig = new ProfileServiceDatabaseConfiguration(Configuration, app);
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

            // Setup http to https redirection
            app.UseHttpsRedirection();

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{SERVICE_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "Profile Service API");
            });

            logger.LogInformation("Waiting for rabbitmq...");
            // Block until the rabbitmq panel is online
            NetworkingHelpers.WaitForOk(new Uri("http://rabbitmq:15672")).Wait();

            // Setup MVC with a Default Route
            //app.UseMvcWithDefaultRoute();
            app.UseMvc();
        }
    }
}
