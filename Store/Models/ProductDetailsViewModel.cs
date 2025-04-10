// Controllers/ProductsController.cs
namespace Store.Models
{
    internal class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string StockStatus { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductReview> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsInWishlist { get; set; }
    }
}