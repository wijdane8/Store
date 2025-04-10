// Services/IEmailService.cs
using System.Threading.Tasks;
using Store.Models; // Adjust namespace

namespace Store.Services // Adjust namespace
{
    public interface IEmailService
    {
        Task SendContactFormEmailAsync(ContactFormModel model);
    }
}