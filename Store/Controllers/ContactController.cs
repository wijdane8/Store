using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Store.Models; // Adjust namespace
using Store.Services; // Adjust namespace

namespace Store.Controllers // Adjust namespace
{
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IEmailService emailService, ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Create this view (Contact/Index.cshtml)
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Important for security
        public async Task<IActionResult> SubmitForm(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return JSON for AJAX
                return Json(new { success = false, message = "الرجاء تصحيح الأخطاء في النموذج." });
                // Or, for a standard MVC form post:
                //return View("Index", model); // Return to the form with errors
            }

            try
            {
                await _emailService.SendContactFormEmailAsync(model);
                _logger.LogInformation("Contact form submitted by {Email}", model.Email);
                // Return JSON for AJAX
                return Json(new { success = true, message = "تم إرسال رسالتك بنجاح!" });
                // Or, for a standard MVC form post:
                //TempData["SuccessMessage"] = "تم إرسال رسالتك بنجاح!";
                //return RedirectToAction("Index"); // Redirect to clear the form
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                // Return JSON for AJAX
                return Json(new { success = false, message = "فشل إرسال رسالتك. الرجاء المحاولة مرة أخرى لاحقًا." });
                // Or, for a standard MVC form post:
                //ModelState.AddModelError("", "فشل إرسال رسالتك. الرجاء المحاولة مرة أخرى لاحقًا.");
                //return View("Index", model); // Return to the form with errors
            }
        }
    }
}