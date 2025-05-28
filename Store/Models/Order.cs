using System;
using System.Collections.Generic;

namespace Store.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; } 

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } 

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}