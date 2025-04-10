using Microsoft.EntityFrameworkCore;

namespace Store.Data // Or your preferred namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Your DbSet properties will go here, e.g.,
        // public DbSet<Product> Products { get; set; }
    }
}