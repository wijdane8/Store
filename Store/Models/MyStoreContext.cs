// Store/Models/MyStoreContext.cs

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Store.Models; // تأكد من هذا الـ using
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // *** أضف هذا الـ using ***

namespace Store.Models // أو namespace Store.Data إذا كانت في مجلد Data
{
    // MyStoreContext should inherit from IdentityDbContext<ApplicationUser>
    // if you are using ASP.NET Core Identity and want to manage users/roles
    // through this context.
    public partial class MyStoreContext : IdentityDbContext<ApplicationUser>
    {
        public MyStoreContext()
        {
        }

        public MyStoreContext(DbContextOptions<MyStoreContext> options)
            : base(options)
        {
        }

        // --- Existing DbSets ---
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductNotification> ProductNotifications { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }

        // --- NEW DbSets for Orders and OrderItems ---
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // هذا فارغ هنا، لأن سلسلة الاتصال يتم تكوينها في Program.cs
            // تأكد من أنك تقوم بتكوين سلسلة الاتصال في Program.cs أو Startup.cs بشكل صحيح
            // مثال:
            // services.AddDbContext<MyStoreContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // *** مهم جداً: يجب استدعاء base.OnModelCreating(modelBuilder) أولاً ***
            // هذا ضروري لتكوينات IdentityDbContext الخاصة بـ Identity tables (Users, Roles, etc.)
            base.OnModelCreating(modelBuilder);

            // يمكنك إزالة HasName() من تكوينات العلاقات إذا كان EF Core يولدها تلقائياً
            // وتجنب إعادة تعريف الجداول الأساسية لـ Identity (AspNetUser, AspNetRole, إلخ)

            // --- Configure decimal precision for ALL decimal properties ---
            // This is the primary fix for the "No store type was specified for the decimal property" warning.
            // Using HasColumnType is explicit and directly sets the SQL type.
            // You can also use .Property(p => p.Price).HasPrecision(18, 2); if you prefer.

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.OldPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(18, 2)"); // Assuming Weight can also be decimal

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");


            // --- Entity Configurations and Relationships ---

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                // Consider if UserId should be unique for carts or just an index
                // If a user can have multiple carts (e.g., historical active carts), remove .IsUnique()
                // If only one 'Active' cart per user, then this is fine, but you'd need logic to enforce status
                // If it's a "current active cart", then it's common to make UserId unique.
                entity.HasIndex(e => e.UserId).IsUnique(); // Keep unique if one active cart per user is enforced

                entity.HasOne<ApplicationUser>() // Specify the principal entity type explicitly
                      .WithMany() // Can be WithMany(u => u.Carts) if you add ICollection<Cart> to ApplicationUser
                      .HasForeignKey(c => c.UserId)
                      .IsRequired(); // A cart must belong to a user

                // Map the Status property to an int column in the database
                entity.Property(e => e.Status)
                      .HasColumnType("int")
                      .IsRequired();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique(); // Ensures one type of product per cart
                // Price configuration already added above
                entity.HasOne(d => d.Cart)
                      .WithMany(p => p.CartItems)
                      .HasForeignKey(d => d.CartId)
                      .IsRequired(); // A cart item must belong to a cart

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.CartItems) // Assuming Product has ICollection<CartItem> CartItems
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired(); // A cart item must reference a product
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Brand).HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Dimensions).HasMaxLength(100);
                entity.Property(e => e.Material).HasMaxLength(100);
                entity.Property(e => e.Name).HasMaxLength(255);
                // Price and OldPrice configs added above
                entity.Property(e => e.Sku).HasMaxLength(50).HasColumnName("SKU");
                entity.Property(e => e.Warranty).HasMaxLength(255);
                // Weight config added above
                entity.Property(e => e.AverageRating).HasColumnType("float"); // Double in C# maps to float in SQL
                entity.Property(e => e.ReviewCount).IsRequired(); // int ReviewCount

                entity.HasOne(d => d.Cat)
                      .WithMany(p => p.Products) // Assuming Category has ICollection<Product> Products
                      .HasForeignKey(d => d.CatId);
                // CatId is nullable, so no .IsRequired() on the FK if it can be null
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsMain).HasDefaultValue(false);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired(); // An image must belong to a product
            });

            modelBuilder.Entity<ProductNotification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ProductId, e.Email }).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.NotificationDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductNotifications)
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.UserId).HasMaxLength(450);
                entity.HasOne(d => d.User)
                      .WithMany(u => u.ProductReviews) // Assuming ApplicationUser has ICollection<ProductReview> ProductReviews
                      .HasForeignKey(d => d.UserId)
                      .IsRequired();
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductReviews)
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                entity.Property(e => e.AddedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.Wishlists) // Assuming Product has ICollection<Wishlist> Wishlists
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
                entity.HasOne(d => d.User)
                      .WithMany(u => u.Wishlists) // Assuming ApplicationUser has ICollection<Wishlist> Wishlists
                      .HasForeignKey(d => d.UserId)
                      .IsRequired();
            });

            // --- New Entities: Order and OrderItem Configuration ---

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).HasColumnType("datetime2").IsRequired();
                // TotalAmount precision already set above
                entity.Property(e => e.Status) // Map the OrderStatus enum to an int column
                      .HasColumnType("int")
                      .IsRequired();

                entity.HasOne<ApplicationUser>() // Order belongs to an ApplicationUser
                      .WithMany() // You might add ICollection<Order> to ApplicationUser if needed
                      .HasForeignKey(e => e.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                // Price precision already set above
                entity.Property(e => e.Quantity).IsRequired(); // Ensure quantity is required

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderItems) // Order has many OrderItems
                      .HasForeignKey(d => d.OrderId)
                      .IsRequired();

                entity.HasOne(d => d.Product)
                      .WithMany() // Product has many OrderItems, but often no direct nav prop on Product model for it.
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}