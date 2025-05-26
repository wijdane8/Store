// Store/Models/ContactFormModel.cs
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; } = null!; // حل CS8618
        [Required]
        public string Subject { get; set; } = null!; // حل CS8618
        [Required]
        public string Message { get; set; } = null!; // حل CS8618

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!; // تأكد أنها مهيأة أيضًا

        // يمكنك إضافة خصائص أخرى مثل رقم الهاتف، إلخ.
    }
}