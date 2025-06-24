
using Microsoft.AspNetCore.Identity;

namespace MoneyTracker.Models
{
    public class User : IdentityUser<int>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool IsDeleted { get; set; }
    }
}