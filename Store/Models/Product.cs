// File: Store/Models/Product.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(255, ErrorMessage = "يجب أن يكون اسم المنتج بين {2} و {1} حرفًا.", MinimumLength = 3)]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; } = null!; // Non-nullable, default init to avoid CS8618

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من 0")]
        [Column(TypeName = "decimal(18, 2)")] // Explicitly define decimal precision
        [Display(Name = "السعر")]
        public decimal Price { get; set; } // Non-nullable decimal

        [Display(Name = "صورة المنتج")]
        public string? Photo { get; set; } // Consider moving to ProductImage collection for multiple images

        // *** الاسم الموحد للخاصية ***
        [Display(Name = "الكمية المتوفرة")]
        [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية رقمًا موجبًا")]
        public int? StockQuantity { get; set; } // Nullable int

        // السعر القديم (إذا كان هناك خصم)، يمكن أن يكون قابل للقيم الخالية
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "السعر القديم")]
        public decimal? OldPrice { get; set; } // Nullable decimal

        [Display(Name = "رمز المنتج (SKU)")]
        [StringLength(50)]
        public string? Sku { get; set; }

        [Display(Name = "وصف مختصر")]
        [StringLength(250)]
        public string? ShortDescription { get; set; }

        [Display(Name = "العلامة التجارية")]
        [StringLength(100)]
        public string? Brand { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "الوزن")]
        public decimal? Weight { get; set; } // Nullable decimal

        [Display(Name = "الأبعاد")]
        [StringLength(100)]
        public string? Dimensions { get; set; }

        [Display(Name = "اللون")]
        [StringLength(50)]
        public string? Color { get; set; }

        [Display(Name = "المادة")]
        [StringLength(50)]
        public string? Material { get; set; }

        [Display(Name = "الضمان")]
        [StringLength(255)]
        public string? Warranty { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow; // Default value

        [Display(Name = "متاح للشراء")]
        public bool IsAvailable { get; set; } = true; // Default value

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; } = false; // Default value

        // خصائص التقييم
        [Display(Name = "متوسط التقييم")]
        [Column(TypeName = "float")] // Or "double" if needed, ensure consistency with DB
        public double AverageRating { get; set; } = 0.0;

        [Display(Name = "عدد التقييمات")]
        public int ReviewCount { get; set; } = 0;

        // علاقة مع Category
        [Display(Name = "الفئة")]
        public int? CatId { get; set; }
        [ForeignKey("CatId")]
        public virtual Category? Cat { get; set; }

        // Navigation properties (تهيئة المجموعات لتجنب CS8618 و NullReferenceException)
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductNotification> ProductNotifications { get; set; } = new List<ProductNotification>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}