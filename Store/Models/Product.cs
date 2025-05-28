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
        public string Name { get; set; } = null!;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من 0")]
        [Column(TypeName = "decimal(18, 2)")] 
        [Display(Name = "السعر")]
        public decimal Price { get; set; } 

        [Display(Name = "صورة المنتج")]
        public string? Photo { get; set; } 
        [Display(Name = "الكمية المتوفرة")]
        [Range(0, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية رقمًا موجبًا")]
        public int? StockQuantity { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "السعر القديم")]
        public decimal? OldPrice { get; set; }

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
        public decimal? Weight { get; set; } 

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
        public DateTime DateAdded { get; set; } = DateTime.UtcNow; 

        [Display(Name = "متاح للشراء")]
        public bool IsAvailable { get; set; } = true; 

        [Display(Name = "مميز")]
        public bool IsFeatured { get; set; } = false; 

        [Display(Name = "متوسط التقييم")]
        [Column(TypeName = "float")] 
        public double AverageRating { get; set; } = 0.0;

        [Display(Name = "عدد التقييمات")]
        public int ReviewCount { get; set; } = 0;

        [Display(Name = "الفئة")]
        public int? CatId { get; set; }
        [ForeignKey("CatId")]
        public virtual Category? Cat { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductNotification> ProductNotifications { get; set; } = new List<ProductNotification>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public bool IsActive { get; set; } = true;
        public bool UserHasPurchased { get; set; } = false;
    }
}