using System.ComponentModel.DataAnnotations;
namespace Store.Areas.Identity.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "اسم المستخدم/البريد الإلكتروني")]
        public string UserNameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Display(Name = "تذكرني")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}