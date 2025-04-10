// Services/EmailService.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Store.Models;
using MimeKit.Text; // Adjust namespace

namespace Store.Services // Adjust namespace
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendContactFormEmailAsync(ContactFormModel model)
        {
            // Get email settings
            var smtpServer = _configuration.GetValue<string>("SmtpSettings:Server");
            var smtpPort = _configuration.GetValue<int>("SmtpSettings:Port");
            var smtpUser = _configuration.GetValue<string>("SmtpSettings:User");
            var smtpPassword = _configuration.GetValue<string>("SmtpSettings:Password");
            var toEmail = _configuration.GetValue<string>("EmailSettings:ContactFormRecipient");
            var fromEmail = _configuration.GetValue<string>("EmailSettings:ContactFormSender");

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, true);
                    await client.AuthenticateAsync(smtpUser, smtpPassword);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Contact Form", fromEmail));
                    message.To.Add(new MailboxAddress("Recipient", toEmail));
                    message.Subject = $"Contact Form: {model.Subject}";
                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text =
                            $"<p>Name: {model.Name}</p><p>Email: {model.Email}</p><p>Message: {model.Message}</p>"
                    };

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                throw;
            }
        }
    }
}