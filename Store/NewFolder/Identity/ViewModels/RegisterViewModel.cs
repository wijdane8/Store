// في ملف Store/Areas/Identity/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Store.Areas.Identity.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        [Compare("Password", ErrorMessage = "كلمات المرور غير متطابقة.")]
        public string ConfirmPassword { get; set; }

        // إضافة الخاصية المطلوبة
        public string ReturnUrl { get; set; }
    }
}