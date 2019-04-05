using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GakkoServices.APIGateway.Models.GraphQL;
using GakkoServices.Core.Services;
using GakkoServices.Core.Helpers;
using GraphiQl;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RawRabbit;
using RawRabbit.vNext;
using GakkoServices.Core.Middleware;

namespace GakkoServices.APIGateway
{
    public class Startup
    {
        public const string SERVICE_ENDPOINT_REWRITE = "api-gateway";
        public const string CORS_POLICY = "default";
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = environment;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Cors
            // services.AddCors(options =>
            // {
            //     options.AddPolicy(Startup.CORS_POLICY, policy =>
            //     {
            //         policy
            //             .AllowAnyOrigin()
            //             .AllowAnyHeader()
            //             .AllowAnyMethod();
            //     });
            // });
            // services.AddCors();

            // Add MVCCore
            services.AddMvcCore()
                .AddApiExplorer()
                .AddAuthorization()
                .AddJsonFormatters()
                // .AddCors()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Setup Authentication
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "http://localhost:5001";
                    options.RequireHttpsMetadata = false;
                    options.Audience = "api1";
                });

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "API Gateway API", Version = "v1" });
            });

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

            // Additional Configuration
            services.AddHttpContextAccessor();
            services.AddSingleton<ContextServiceLocator>();
            GraphQLConfiguration.Configure(services);

            // Build the GraphQL Schema
            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new APIGatewaySchema(new FuncDependencyResolver(type => sp.GetService(type))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Change the Root Path of the Service
            app.UsePathBase($"/{SERVICE_ENDPOINT_REWRITE}");

            // Configure our Error Pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable custom Middleware to force Cross Site Access
            app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder()
                .AddDefaultSecurePolicy()
                //.AddCustomHeader("Access-Control-Allow-Origin", "http://localhost:3000")
                //.AddCustomHeader("Access-Control-Allow-Methods", "OPTIONS, GET, POST, PUT, PATCH, DELETE")
                //.AddCustomHeader("Access-Control-Allow-Headers", "X-PINGOTHER, Content-Type, Authorization")
                .AddCustomHeader("X-Developer-Message", "<3 Team Tomodachi"));

            // Enable Swagger Middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{SERVICE_ENDPOINT_REWRITE}/swagger/v1/swagger.json", "API Gateway API");
            });

            // Enable GraphiQL
            app.UseGraphiQl($"/graphiql", $"/{SERVICE_ENDPOINT_REWRITE}/api/graphql");

            // Enable CORS
            // app.UseCors(Startup.CORS_POLICY);
            // app.UseCors(
            //     options => options.AllowAnyOrigin()//.WithOrigins("http://localhost:3000")
            //     .AllowAnyMethod()
            //     .AllowAnyHeader()
            // );

            // Use Authentication
            app.UseAuthentication();

            // Setup MVC with a Default Route
            app.UseMvc();
        }
    }
}
