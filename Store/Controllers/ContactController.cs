using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Store.Models; 
using Store.Services; 

namespace Store.Controllers 
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
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> SubmitForm(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                
                return Json(new { success = false, message = "الرجاء تصحيح الأخطاء في النموذج." });
                
            }

            try
            {
                await _emailService.SendContactFormEmailAsync(model);
                _logger.LogInformation("Contact form submitted by {Email}", model.Email);
                return Json(new { success = true, message = "تم إرسال رسالتك بنجاح!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return Json(new { success = false, message = "فشل إرسال رسالتك. الرجاء المحاولة مرة أخرى لاحقًا." });
            }
        }
    }
}