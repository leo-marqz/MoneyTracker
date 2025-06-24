using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.DTOs
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}