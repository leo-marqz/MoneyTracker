
using System.Threading.Tasks;

namespace MoneyTracker.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SMTPConfiguration _smtpConfig;

        public EmailService(SMTPConfiguration smtpConfig)
        {
            _smtpConfig = smtpConfig;
        }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            throw new System.NotImplementedException();
        }

        public Task SendEmailConfirmationAsync(string to, string callbackUrl)
        {
            throw new System.NotImplementedException();
        }

        public Task SendPasswordResetAsync(string to, string callbackUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}