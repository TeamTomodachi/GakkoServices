using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.Core.Services;
using GakkoServices.Core.Helpers;
using GakkoServices.Microservices.MetadataService.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Hosting = Microsoft.Extensions.Hosting;
using GakkoServices.Microservices.MetadataService.BackgroundServices;
using RawRabbit;
using RawRabbit.vNext;
using GakkoServices.Microservices.MetadataService.Services;

namespace GakkoServices.Microservices.MetadataService
{
    public class Startup
    {
        const string SERVICE_ENDPOINT_REWRITE = "metadata";

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
            MetadataDatabaseConfiguration databaseConfig = new MetadataDatabaseConfiguration(Configuration, null);

            // Configure DBContexts
            services.AddDbContext<MetadataServiceDbContext>(options => databaseConfig.BuildDBContext(options));

            services.AddRawRabbit(options =>
            {
                options.SetBasePath(Environment.ContentRootPath)
                    .AddJsonFile("rawrabbit.json")
                    .AddEnvironmentVariables("RawRabbit:");
            });

            // Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Profile Service API", Version = "v1" });
            });

            // Additional Configuration
            services.AddHttpContextAccessor();
            services.AddSingleton<ContextServiceLocator>();
            services.AddSingleton<Hosting.IHostedService, MessageHandlerService>();
            services.AddSingleton<PokeApiService, PokeApiService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Change the Root Path of the Profile
            app.UsePathBase($"/{SERVICE_ENDPOINT_REWRITE}");

            // Initialize our Databases
            try
            {
                MetadataDatabaseConfiguration databaseConfig = new MetadataDatabaseConfiguration(Configuration, app);
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

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{SERVICE_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "Profile Service API");
            });

            _logger.LogInformation("Waiting for rabbitmq...");
            // Block until the rabbitmq panel is online
            NetworkingHelpers.WaitForOk(new Uri("http://rabbitmq:15672")).Wait();
            _logger.LogInformation("rabbitmq is ready");

            // Setup MVC with a Default Route
            //app.UseMvcWithDefaultRoute();
            app.UseMvc();
        }
    }
}
