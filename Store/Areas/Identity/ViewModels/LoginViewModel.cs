using System.ComponentModel.DataAnnotations; // Add this using directive

namespace Store.Areas.Identity.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserNameOrEmail is required")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}