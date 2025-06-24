
using System;

namespace MoneyTracker.Services.JwtToken
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}