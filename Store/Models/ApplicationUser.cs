using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation properties
        public Cart Cart { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}