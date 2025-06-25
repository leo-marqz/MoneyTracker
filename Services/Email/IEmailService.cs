
using System.Threading.Tasks;

namespace MoneyTracker.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailByConfirmationEmailAsync(string to, string confirmationLink);
    }
}