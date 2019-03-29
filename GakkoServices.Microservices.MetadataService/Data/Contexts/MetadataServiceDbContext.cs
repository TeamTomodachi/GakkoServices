using GakkoServices.Microservices.MetadataService.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.MetadataService.Data.Contexts
{
    public class MetadataServiceDbContext : DbContext
    {
        public MetadataServiceDbContext(DbContextOptions options) : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }

        public DbSet<PogoTeam> PogoTeams { get; set; }
        public DbSet<PogoPokemon> PogoPokemon { get; set; }
        public DbSet<PogoPokemonType> PogoPokemonTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
