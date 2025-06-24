
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MoneyTracker.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailService(IOptions<SMTPConfiguration> smtpConfig, ILogger<EmailService> logger)
        {
            _logger = logger;

            if (smtpConfig == null || smtpConfig.Value == null)
            {
                throw new ArgumentNullException(nameof(smtpConfig), "SMTP configuration cannot be null.");
            }

            var smtp = smtpConfig.Value;

            try
            {
                _smtpClient = new SmtpClient
                {
                    Host = smtp.Host,
                    Port = smtp.Port,
                    Credentials = new NetworkCredential(smtp.Username, smtp.Password),
                    EnableSsl = smtp.EnableSSL
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize SMTP client with provided configuration.");
            }
        }

        
        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            SendEmailAsyncValidator(to, subject, body);

            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@moneytracker.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            return _smtpClient.SendMailAsync(mailMessage);
        }

        private void SendEmailAsyncValidator(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
            }
        }
    }
}