// Store/Models/OrderItem.cs
namespace Store.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } // Navigation property

        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation property

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at the time of purchase
    }
}