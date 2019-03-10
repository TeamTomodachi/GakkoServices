using GakkoServices.Microservices.ProfileService.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.Microservices.ProfileService.Data.Contexts
{
    public class ProfileServiceDbContext : DbContext
    {
        public ProfileServiceDbContext(DbContextOptions options) : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
        }

        public DbSet<PogoProfile> PogoProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
