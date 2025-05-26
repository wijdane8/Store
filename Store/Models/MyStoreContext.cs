// Store.Models/MyStoreContext.cs
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Store.Models; // تأكد من هذا الـ using
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // *** أضف هذا الـ using ***

namespace Store.Models // أو namespace Store.Data إذا كانت في مجلد Data
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // هذا فارغ هنا، لأن سلسلة الاتصال يتم تكوينها في Program.cs
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // *** مهم جداً: يجب استدعاء base.OnModelCreating(modelBuilder) أولاً ***
            base.OnModelCreating(modelBuilder);

            // يمكنك إزالة HasName() من تكوينات العلاقات إذا كان EF Core يولدها تلقائياً
            // وتجنب إعادة تعريف الجداول الأساسية لـ Identity (AspNetUser, AspNetRole, إلخ)

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique(); // التأكد من أن هذا Index صحيح إذا كان UserId فريداً
                entity.HasOne<ApplicationUser>()
                      .WithMany() // يمكنك استخدام .WithMany(u => u.Carts) إذا أضفت ICollection<Cart> إلى ApplicationUser
                      .HasForeignKey(c => c.UserId)
                      .IsRequired();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => new { e.CartId, e.ProductId }).IsUnique();
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId);
                entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductId);
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
                entity.Property(e => e.OldPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Sku).HasMaxLength(50).HasColumnName("SKU");
                entity.Property(e => e.Warranty).HasMaxLength(255);
                entity.Property(e => e.Weight).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.AverageRating).HasColumnType("float");
                entity.Property(e => e.ReviewCount).IsRequired();

                entity.HasOne(d => d.Cat).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatId);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsMain).HasDefaultValue(false);
                entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId);
            });

            modelBuilder.Entity<ProductNotification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ProductId, e.Email }).IsUnique();
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.NotificationDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.HasOne(d => d.Product).WithMany(p => p.ProductNotifications)
                    .HasForeignKey(d => d.ProductId);
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.UserId).HasMaxLength(450);
                entity.HasOne(d => d.User).WithMany(u => u.ProductReviews)
                    .HasForeignKey(d => d.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
                entity.Property(e => e.AddedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.ProductId);
                entity.HasOne(d => d.User).WithMany(u => u.Wishlists)
                    .HasForeignKey(d => d.UserId)
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}