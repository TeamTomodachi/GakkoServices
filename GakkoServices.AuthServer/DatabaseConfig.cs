using GakkoServices.AuthServer.Data.Contexts;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
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

namespace GakkoServices.AuthServer
{
    public static class DatabaseConfig
    {
        public enum SupportedDatabaseServerEngines
        {
            None = 0,
            MSSQL = 1,
            Postgresql = 2
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            string connectionString = configuration["dbConnectionString"];
            return connectionString;
        }

        public static SupportedDatabaseServerEngines GetDatabaseServerEngine(IConfiguration configuration)
        {
            string engine = configuration["dbServerEngine"];
            switch (engine?.ToLower())
            {
                case "1":
                case "mssql":
                    return SupportedDatabaseServerEngines.MSSQL;
                case "2":
                case "postgresql":
                    return SupportedDatabaseServerEngines.Postgresql;
                case "0":
                default: return SupportedDatabaseServerEngines.None;
            }
        }

        public static string GetDatabaseUsername(IConfiguration configuration)
        {
            string username = configuration["dbUsername"];
            return username;
        }

        public static string GetDatabasePassword(IConfiguration configuration)
        {
            string password = configuration["dbPassword"];
            return password;
        }

        public static void BuildDBContext(DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = GetConnectionString(configuration);
            var dbServerEngine = GetDatabaseServerEngine(configuration);

            switch (dbServerEngine)
            {
                case SupportedDatabaseServerEngines.MSSQL:
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.Postgresql:
                    options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.None:
                default:
                    break;
            }
        }

        public static void InitializeDatabase(IApplicationBuilder app)
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
                    foreach (var client in IdentityServerConfig.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityServerConfig.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in IdentityServerConfig.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
