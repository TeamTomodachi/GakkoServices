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

namespace GakkoServices.Core.Configuration
{
    public abstract partial class DatabaseConfiguration
    {
        protected IConfiguration Configuration { get; }
        protected IApplicationBuilder ApplicationBuilder { get; }
        public Assembly MigrationAssembly { get; set; }

        public string dbUsernameConfigurationKey { get; set; } = "dbUsername";
        public string dbPasswordConfigurationKey { get; set; } = "dbPassword";
        public string dbConnectionStringConfigurationKey { get; set; } = "dbConnectionString";
        public string dbServerEngineConfigurationKey { get; set; } = "dbServerEngine";

        public DatabaseConfiguration(IConfiguration configuration, Assembly migrationAssembly)
        {
            Configuration = configuration;
            MigrationAssembly = migrationAssembly;
        }

        public DatabaseConfiguration(IConfiguration configuration, IApplicationBuilder app, Assembly migrationAssembly) :this(configuration, migrationAssembly)
        {
            ApplicationBuilder = app;
        }

        public virtual string GetConnectionString()
        {
            string connectionString = Configuration[dbConnectionStringConfigurationKey];
            return connectionString;
        }

        public virtual SupportedDatabaseServerEngines GetDatabaseServerEngine()
        {
            string engine = Configuration[dbServerEngineConfigurationKey];
            switch (engine?.ToLower())
            {
                case "1":
                case "mssql":
                    return SupportedDatabaseServerEngines.MSSQL;
                case "2":
                case "postgresql":
                    return SupportedDatabaseServerEngines.PostgreSQL;
                case "3":
                case "mysql":
                    return SupportedDatabaseServerEngines.MySQL;
                case "4":
                case "sqlite":
                    return SupportedDatabaseServerEngines.SQLite;
                case "0":
                default: return SupportedDatabaseServerEngines.None;
            }
        }

        public virtual string GetDatabaseUsername()
        {
            string username = Configuration[dbUsernameConfigurationKey];
            return username;
        }

        public virtual string GetDatabasePassword()
        {
            string password = Configuration[dbPasswordConfigurationKey];
            return password;
        }

        public virtual void BuildDBContext(DbContextOptionsBuilder options)
        {
            var migrationsAssembly = MigrationAssembly.GetName().Name;
            var connectionString = GetConnectionString();
            var dbServerEngine = GetDatabaseServerEngine();

            switch (dbServerEngine)
            {
                case SupportedDatabaseServerEngines.MSSQL:
                    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.PostgreSQL:
                    options.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.MySQL:
                    options.UseMySQL(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.SQLite:
                    options.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    break;
                case SupportedDatabaseServerEngines.None:
                default:
                    break;
            }
        }

        public void InitializeDatabase() { InitializeDatabase(ApplicationBuilder); }
        public abstract void InitializeDatabase(IApplicationBuilder app);
    }
}
