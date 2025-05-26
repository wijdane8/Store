// Store.Services/IEmailService.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services; // تأكد من هذا الـ using
using Store.Models; // تأكد من هذا الـ using لـ ContactFormModel

namespace Store.Services
{
    // اجعل IEmailService ترث من IEmailSender
    public interface IEmailService : IEmailSender
    {
        Task SendContactFormEmailAsync(ContactFormModel model);
    }
}