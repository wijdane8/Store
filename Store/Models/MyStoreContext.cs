using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Store.Models;

public partial class MyStoreContext : DbContext
{
    public MyStoreContext()
    {
    }

    public MyStoreContext(DbContextOptions<MyStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

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
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasDefaultValue("");
            entity.Property(e => e.LastName).HasDefaultValue("");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carts__3214EC075EEC7A6B");

            entity.HasIndex(e => e.UserId, "UQ__Carts__1788CC4D665CDC9E").IsUnique();

            entity.HasOne(d => d.User).WithOne(p => p.Cart).HasForeignKey<Cart>(d => d.UserId);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC070EF1F339");

            entity.HasIndex(e => e.ProductId, "IX_CartItems_ProductId");

            entity.HasIndex(e => new { e.CartId, e.ProductId }, "UQ__CartItem__9AFC1BDA45E59E96").IsUnique();

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CartItems__CartI__4F7CD00D");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CartItems__Produ__5070F446");
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
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.Warranty).HasMaxLength(255);
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 3)");

            entity.HasOne(d => d.Cat).WithMany(p => p.Products)
                .HasForeignKey(d => d.CatId)
                .HasConstraintName("FK__Products__CatId__3A81B327");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC07F477BF69");

            entity.Property(e => e.IsMain).HasDefaultValue(false);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductIm__Produ__3E52440B");
        });

        modelBuilder.Entity<ProductNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductN__3214EC07D6C4223E");

            entity.HasIndex(e => new { e.ProductId, e.Email }, "UQ__ProductN__FE91D69F9A05C036").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.NotificationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductNotifications)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductNo__Produ__5629CD9C");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductR__3214EC07ADED646E");

            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductRe__Produ__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC078B913EE1");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ__Wishlist__DCC80021416BCF6C").IsUnique();

            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Wishlists__Produ__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
