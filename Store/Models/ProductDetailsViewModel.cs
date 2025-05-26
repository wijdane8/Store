// Store/Models/ProductDetailsViewModel.cs (إذا كان موجودًا)
using System.Collections.Generic;

namespace Store.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!; // حل CS8618
        public string StockStatus { get; set; } = null!; // حل CS8618
        public List<Product> RelatedProducts { get; set; } = new List<Product>(); // حل CS8618
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>(); // حل CS8618
        public List<ProductReview> ProductReviews { get; set; } = new List<ProductReview>(); // حل CS8618
        // تأكد من أن جميع المجموعات والخصائص غير القابلة للقيم الخالية مهيأة
    }
}