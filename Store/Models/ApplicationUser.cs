// File: Store/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        // *** إضافة خصائص المجموعات لـ ApplicationUser لتجنب CS8618 و لتمكين علاقات Many ***
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>(); // إذا كنت تريد علاقة مباشرة من User إلى Cart
       
        public ICollection<Order>? Orders { get; set; } // If you map this
    }
}