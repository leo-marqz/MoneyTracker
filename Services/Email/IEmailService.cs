
using System.Threading.Tasks;

namespace MoneyTracker.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendPasswordResetAsync(string to, string callbackUrl);
        Task SendEmailConfirmationAsync(string to, string callbackUrl);
    }
}