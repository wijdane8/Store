using Microsoft.AspNetCore.Identity;

namespace Store.Models
{
    public class ApplicationUser : IdentityUser
    {
        // يمكنك إضافة خصائص إضافية خاصة بمستخدمي تطبيقك هنا
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // خواص التنقل (سيتم إضافتها لاحقًا في سياق قاعدة البيانات)
        public Cart Cart { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}