
using System.Collections.Generic;

namespace Store.Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!; 
        public string StockStatus { get; set; } = null!; 
        public List<Product> RelatedProducts { get; set; } = new List<Product>(); 
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>(); 
        public List<ProductReview> ProductReviews { get; set; } = new List<ProductReview>(); 
    }
}