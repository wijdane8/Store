using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; } = null!; 
        [Required]
        public string Subject { get; set; } = null!; 
        [Required]
        public string Message { get; set; } = null!; 

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!; 

    }
}