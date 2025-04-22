using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Areas.Identity.ViewModels;
using System.Threading.Tasks;
using Store.Models;
using Store.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace Store.Areas.Identity.Controllers
{
    [Area("Identity")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // --- Login Actions ---

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            Console.WriteLine(returnUrl);
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var userName = model.UserNameOrEmail;

                // Improved email validation using Regex
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (emailRegex.IsMatch(userName))
                {
                    var user = await _userManager.FindByEmailAsync(userName);
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                    else
                    {
                        // Generic error message for security
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        _logger.LogWarning("Login attempt failed for email '{Email}'", model.UserNameOrEmail);
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(
                    userName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true); // Enable lockout on failure

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{UserName}' logged in.", userName);
                    return RedirectToLocal(model.ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new
                    {
                        returnUrl = model.ReturnUrl,
                        rememberMe = model.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account '{UserName}' locked out.", userName);
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _logger.LogWarning("Invalid login attempt for '{UserName}'", userName);
                }
            }

            return View(model);
        }

        // --- New Two-Factor Authentication Action ---
        [HttpGet]
        public IActionResult LoginWith2fa(string returnUrl, bool rememberMe)
        {
            // Implement 2FA logic here
            return View(new LoginWith2faViewModel
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            });
        }

        // --- Logout Action ---

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "" });
        }

        // --- Registration Actions ---

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            Console.WriteLine(returnUrl);
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            Console.WriteLine(ModelState.IsValid);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{UserName}' created account.", user.UserName);

                    // Consider sending confirmation email before sign-in
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            // Preserve returnUrl when redisplaying the form
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        // --- Helper Method ---

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "" });
        }

        // --- Additional Recommended Actions ---
        // Implement Email Confirmation, Password Recovery, etc.
    }
}