using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Areas.Identity.ViewModels; // Assuming your ViewModels are in this namespace
using System.Threading.Tasks;
using Store.Models; // Assuming your ApplicationUser is in the Models namespace
using System.ComponentModel.DataAnnotations; // Add this for [Required]

namespace Store.Areas.Identity.Controllers // IMPORTANT: Update the namespace to be within the Area
{
    [Area("Identity")] // Add this attribute to associate with the Identity Area
    [AllowAnonymous] // Make this controller accessible without login
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent cross-site request forgery
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserNameOrEmail, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(model.ReturnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return View("Lockout"); // Ensure you have a Lockout view in Areas/Identity/Views/Account/
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "محاولة تسجيل الدخول غير صالحة."); // Invalid login attempt
                    return View(model); // This will look for Areas/Identity/Views/Account/Login.cshtml
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model); // This will look for Areas/Identity/Views/Account/Login.cshtml
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home"); // Use string literals for better refactoring
            }
        }

        // Implement a Logout action if needed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home"); // Use string literals
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(); // This will look for Areas/Identity/Views/Account/Register.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Redirect after registration
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model); // This will look for Areas/Identity/Views/Account/Register.cshtml
        }
    }
}