using GakkoServices.AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GakkoServices.AuthServer.Data.Contexts
{
    public class AspIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AspIdentityDbContext(DbContextOptions<AspIdentityDbContext> options)
            : base(options)
        {
        }

        // Backup if we can't get regular auth to work
        //public DbSet<AuthToken> AuthTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
