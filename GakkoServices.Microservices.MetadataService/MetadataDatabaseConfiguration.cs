using GakkoServices.Core.Configuration;
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

                // Add the default Teams
                if (!context.PogoTeams.Any())
                {
                    context.PogoTeams.Add(new Models.PogoTeam() { Id = Guid.Parse("5523827B-7266-4EE7-8F6D-F4B43C0060E2"), Name = "Mystic", Color = "Blue", ImageUrl = "" });
                    context.PogoTeams.Add(new Models.PogoTeam() { Id = Guid.Parse("4FD050C0-0079-4552-B9E9-F37F2FFA9DAE"), Name = "Valor", Color = "Red", ImageUrl = "" });
                    context.PogoTeams.Add(new Models.PogoTeam() { Id = Guid.Parse("D1681284-2A72-47AB-85C3-FE5CD642FBA9"), Name = "Instinct", Color = "Yellow", ImageUrl = "" });
                    context.SaveChanges();
                }

                // Add Pokemon Types
                if (!context.PogoPokemonTypes.Any())
                {
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Normal" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Fighting" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Flying" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Poison" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Ground" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Rock" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Bug" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Ghost" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Steel" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Fire" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Water" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Grass" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Electric" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Psychic" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Ice" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Dragon" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Fairy" });
                    context.PogoPokemonTypes.Add(new Models.PogoPokemonType() { Id = Guid.NewGuid(), Name = "Dark" });
                    context.SaveChanges();
                }

                // Add some Pokemon
                if (!context.PogoPokemon.Any())
                {
                    context.SaveChanges();
                }
            }
        }
    }
}
