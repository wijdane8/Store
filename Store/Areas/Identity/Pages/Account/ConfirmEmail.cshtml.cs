// Store/Areas/Identity/Pages/Account/ConfirmEmail.cshtml.cs
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Store.Models; // تأكد من أن هذا الـ using يشير إلى ApplicationUser الخاص بك

namespace Store.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string? StatusMessage { get; set; } // اجعلها قابلة للقيمة الفارغة (nullable)

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                StatusMessage = "شكرًا لك على تأكيد بريدك الإلكتروني. يمكنك الآن تسجيل الدخول.";
                // *** إضافة هذا السطر لإعادة التوجيه إلى صفحة تسجيل الدخول ***
                return RedirectToPage("./Login"); // أو "/Account/Login" إذا كانت في مسار مختلف
            }
            else
            {
                StatusMessage = "خطأ في تأكيد بريدك الإلكتروني.";
            }

            // إذا أردت عرض رسالة ConfirmEmail.cshtml بدلاً من إعادة التوجيه
            // يمكنك إزالة سطر RedirectToPage("./Login"); وستظهر الرسالة الافتراضية
            return Page();
        }
    }
}