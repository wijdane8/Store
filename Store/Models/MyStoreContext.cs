using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Store.Models; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Store.Models 
{
    public partial class MyStoreContext : IdentityDbContext<ApplicationUser>
    {
        public MyStoreContext()
        {
        }

        public MyStoreContext(DbContextOptions<MyStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductNotification> ProductNotifications { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // هذا فارغ هنا، لأن سلسلة الاتصال يتم تكوينها في Program.cs
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.OldPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");


            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne<ApplicationUser>() 
                      .WithMany() 
                      .HasForeignKey(c => c.UserId)
                      .IsRequired(); 

                entity.Property(e => e.Status)
                      .HasColumnType("int")
                      .IsRequired();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique(); 
                
                entity.HasOne(d => d.Cart)
                      .WithMany(p => p.CartItems)
                      .HasForeignKey(d => d.CartId)
                      .IsRequired(); 

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.CartItems) 
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
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
                entity.Property(e => e.Sku).HasMaxLength(50).HasColumnName("SKU");
                entity.Property(e => e.Warranty).HasMaxLength(255);
                entity.Property(e => e.AverageRating).HasColumnType("float"); 
                entity.Property(e => e.ReviewCount).IsRequired();

                entity.HasOne(d => d.Cat)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CatId);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsMain).HasDefaultValue(false);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired(); 
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
                      .WithMany(u => u.ProductReviews) 
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
                      .WithMany(p => p.Wishlists) 
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
                entity.HasOne(d => d.User)
                      .WithMany(u => u.Wishlists) 
                      .HasForeignKey(d => d.UserId)
                      .IsRequired();
            });

            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).HasColumnType("datetime2").IsRequired();
                entity.Property(e => e.Status) 
                      .HasColumnType("int")
                      .IsRequired();

                entity.HasOne<ApplicationUser>()
                      .WithMany() 
                      .HasForeignKey(e => e.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired(); 

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderItems) 
                      .HasForeignKey(d => d.OrderId)
                      .IsRequired();

                entity.HasOne(d => d.Product)
                      .WithMany() 
                      .HasForeignKey(d => d.ProductId)
                      .IsRequired();
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}