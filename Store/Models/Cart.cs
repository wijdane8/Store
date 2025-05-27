// Store/Models/Cart.cs (مثال، قد يختلف عن ملفك)
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!; // حل CS8618

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public CartStatus Status { get; set; }

    }
}
namespace Store.Models
{
    public enum CartStatus
    {
        Active,
        Pending,
        Processing,
        Completed,
        Cancelled,
        Shipped,
        Delivered
    }
}