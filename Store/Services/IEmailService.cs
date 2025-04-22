using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Store.Services
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);

        // Add the declaration for your contact form email method:
        Task SendContactFormEmailAsync(Store.Models.ContactFormModel model); // Assuming your model is in the Store.Models namespace
    }
}