using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Store.Models;

public partial class MyStoreContext : IdentityDbContext<ApplicationUser> // Inherit from IdentityDbContext
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=MyStore;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important: Call the base implementation first

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carts__3214EC075EEC7A6B");
            entity.HasIndex(e => e.UserId, "UQ__Carts__1788CC4D665CDC9E").IsUnique();
            entity.HasOne(d => d.User).WithOne(p => p.Cart).HasForeignKey<Cart>(d => d.UserId).HasConstraintName("FK_Carts_ApplicationUser_UserId");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC070EF1F339");
            entity.HasIndex(e => e.ProductId, "IX_CartItems_ProductId");
            entity.HasIndex(e => new { e.CartId, e.ProductId }, "UQ__CartItem__9AFC1BDA45E59E96").IsUnique();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasForeignKey(d => d.CartId).HasConstraintName("FK__CartItems__CartI__4F7CD00D");
            entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasForeignKey(d => d.ProductId).HasConstraintName("FK__CartItems__Produ__5070F446");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07F65089DE");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC075E3ADA10");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Dimensions).HasMaxLength(100);
            entity.Property(e => e.Material).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.OldPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Sku).HasMaxLength(50).HasColumnName("SKU");
            entity.Property(e => e.Warranty).HasMaxLength(255);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 3)");
            entity.HasOne(d => d.Cat).WithMany(p => p.Products).HasForeignKey(d => d.CatId).HasConstraintName("FK__Products__CatId__3A81B327");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC07F477BF69");
            entity.Property(e => e.IsMain).HasDefaultValue(false);
            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasForeignKey(d => d.ProductId).HasConstraintName("FK__ProductIm__Produ__3E52440B");
        });

        modelBuilder.Entity<ProductNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductN__3214EC07D6C4223E");
            entity.HasIndex(e => new { e.ProductId, e.Email }, "UQ__ProductN__FE91D69F9A05C036").IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.NotificationDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.HasOne(d => d.Product).WithMany(p => p.ProductNotifications).HasForeignKey(d => d.ProductId).HasConstraintName("FK__ProductNo__Produ__5629CD9C");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC07ADED646E");
            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews).HasForeignKey(d => d.ProductId).HasConstraintName("FK__ProductRe__Produ__4316F928");
            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews).HasForeignKey(d => d.UserId).HasConstraintName("FK_ProductReviews_ApplicationUser_UserId");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC078B913EE1");
            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ__Wishlist__DCC80021416BCF6C").IsUnique();
            entity.Property(e => e.AddedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists).HasForeignKey(d => d.ProductId).HasConstraintName("FK__Wishlists__Produ__47DBAE45");
            entity.HasOne(d => d.User).WithMany(p => p.Wishlists).HasForeignKey(d => d.UserId).HasConstraintName("FK__Wishlists__AspNetUsers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}