// Store/Models/Order.cs
using System;
using System.Collections.Generic;

namespace Store.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Foreign key to ApplicationUser

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } // e.g., Pending, Completed, Cancelled

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}