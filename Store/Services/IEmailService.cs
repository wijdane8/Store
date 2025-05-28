using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Store.Models;

namespace Store.Services
{
    
    public interface IEmailService : IEmailSender
    {
        Task SendContactFormEmailAsync(ContactFormModel model);
    }
}