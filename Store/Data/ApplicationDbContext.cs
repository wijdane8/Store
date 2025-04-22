using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the composite primary key for AspNetUserLogins
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
            });

            // Optional: Customize other Identity tables if needed
            builder.Entity<ApplicationUser>(entity =>
            {
                // Add your custom user configurations here
            });

            // Add other entity configurations...
        }
    }
}