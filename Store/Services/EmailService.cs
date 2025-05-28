using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Store.Models;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Store.Services
{
    public class EmailService : IEmailService, IEmailSender
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
            try
            {
                if (string.IsNullOrEmpty(_emailSettings.FromAddress))
                {
                    _logger.LogError("Email 'FromAddress' is not configured in EmailSettings. Cannot send email.");
                    throw new ArgumentNullException(nameof(_emailSettings.FromAddress), "Email 'FromAddress' is not configured.");
                }
                if (string.IsNullOrEmpty(_smtpSettings.Server) || _smtpSettings.Port == 0)
                {
                    _logger.LogError("SMTP Server or Port is not configured.");
                    throw new InvalidOperationException("SMTP server settings are incomplete.");
                }

                string smtpUsername = _smtpSettings.Username ?? string.Empty;
                string smtpPassword = _smtpSettings.Password ?? string.Empty;

                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    bool enableSslValue;
                    if (_smtpSettings.EnableSsl is bool sslValue) 
                    {
                        enableSslValue = sslValue;
                    }
                    else if (bool.TryParse(_smtpSettings.EnableSsl.ToString(), out sslValue)) 
                    {
                        enableSslValue = sslValue;
                    }
                    else
                    {
                        _logger.LogError("SmtpSettings.EnableSsl could not be converted to boolean. Defaulting to false.");
                        enableSslValue = false;
                    }
                    client.EnableSsl = enableSslValue; 

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    string finalSubject = subject ?? string.Empty;
                    string finalHtmlMessage = htmlMessage ?? string.Empty;

                    var mailMessage = new MailMessage(
                        _emailSettings.FromAddress,
                        email,
                        finalSubject,
                        finalHtmlMessage
                    );
                    mailMessage.IsBodyHtml = true;

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {email} from {_emailSettings.FromAddress}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {email} with subject '{subject}'.");
                throw;
            }
        }

        public async Task SendContactFormEmailAsync(ContactFormModel model)
        {
            string emailSubject = (string)(model.Subject ?? string.Empty);
            string emailBody = $"From: {model.Name} ({model.Email})\n\n{model.Message}";

            try
            {
                if (string.IsNullOrEmpty(_emailSettings.FromAddress))
                {
                    _logger.LogError("Email 'FromAddress' is not configured in EmailSettings for contact form. Cannot send email.");
                    throw new ArgumentNullException(nameof(_emailSettings.FromAddress), "Email 'FromAddress' is not configured for contact form.");
                }
                if (string.IsNullOrEmpty(_emailSettings.ContactRecipientEmail))
                {
                    _logger.LogWarning("Email 'ContactRecipientEmail' is not configured in EmailSettings. Using FromAddress as recipient for contact form.");
                }
                if (string.IsNullOrEmpty(_smtpSettings.Server) || _smtpSettings.Port == 0)
                {
                    _logger.LogError("SMTP Server or Port is not configured for contact form.");
                    throw new InvalidOperationException("SMTP server settings are incomplete for contact form.");
                }

                string smtpUsername = _smtpSettings.Username ?? string.Empty;
                string smtpPassword = _smtpSettings.Password ?? string.Empty;

                using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
                {
                    bool enableSslValue;
                    if (_smtpSettings.EnableSsl is bool sslValue)
                    {
                        enableSslValue = sslValue;
                    }
                    else if (bool.TryParse(_smtpSettings.EnableSsl.ToString(), out sslValue))
                    {
                        enableSslValue = sslValue;
                    }
                    else
                    {
                        _logger.LogError("SmtpSettings.EnableSsl could not be converted to boolean for contact form. Defaulting to false.");
                        enableSslValue = false;
                    }
                    client.EnableSsl = enableSslValue;

                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage(
                        _emailSettings.FromAddress,
                        _emailSettings.ContactRecipientEmail ?? _emailSettings.FromAddress,
                        emailSubject,
                        emailBody
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