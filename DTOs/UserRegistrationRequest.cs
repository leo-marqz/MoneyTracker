
using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.DTOs
{
    public class UserRegistrationRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}