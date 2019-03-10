﻿using GakkoServices.Core.Configuration;
using GakkoServices.Microservices.MetadataService.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService
{
    public class MetadataDatabaseConfiguration : DatabaseConfiguration
    {

        public MetadataDatabaseConfiguration(IConfiguration configuration, IApplicationBuilder app) : base(configuration, app, Assembly.GetExecutingAssembly())
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly;
            MigrationAssembly = migrationsAssembly;
        }

        public override void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // Migrate the ProfileServiceDbContext Context
                var context = serviceScope.ServiceProvider.GetRequiredService<MetadataServiceDbContext>();
                context.Database.Migrate();
            }
        }
    }
}
