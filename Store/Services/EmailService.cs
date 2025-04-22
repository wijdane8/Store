using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Store.Models; // Make sure to include the namespace for ContactFormModel

namespace Store.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<SmtpSettings> smtpSettings, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Your existing SendEmailAsync implementation
            try
            {
                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    var mailMessage = new MailMessage(
                        _emailSettings.FromAddress,
                        email,
                        subject,
                        htmlMessage
                    );
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {email} with subject '{subject}'.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {email} with subject '{subject}'.");
                throw;
            }
        }

        // Implement the SendContactFormEmailAsync method:
        public async Task SendContactFormEmailAsync(ContactFormModel model)
        {
            var subject = "New Contact Form Submission";
            var htmlMessage = $@"
                <p><strong>Name:</strong> {model.Name}</p>
                <p><strong>Email:</strong> {model.Email}</p>
                <p><strong>Subject:</strong> {model.Subject}</p>
                <p><strong>Message:</strong></p>
                <p>{model.Message}</p>
            ";

            try
            {
                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                    client.EnableSsl = _smtpSettings.EnableSsl;

                    var mailMessage = new MailMessage(
                        _emailSettings.FromAddress, // Or perhaps a dedicated admin email
                        _emailSettings.ContactRecipientEmail ?? _emailSettings.FromAddress, // Send to a contact recipient if configured
                        subject,
                        htmlMessage
                    );
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Contact form email sent successfully from {model.Email}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending contact form email from {model.Email}.");
                throw;
            }
        }
    }
}