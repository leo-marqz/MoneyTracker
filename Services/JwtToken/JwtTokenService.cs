
using System;
using Microsoft.Extensions.Options;
using MoneyTracker.Models;

namespace MoneyTracker.Services.JwtToken
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtTokenService(IOptions<JwtConfiguration> jwtConfiguration)
        {
            JwtTokenServiceValidator(jwtConfiguration);

            _jwtConfiguration = jwtConfiguration.Value;
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateToken(User user)
        {
            throw new NotImplementedException();
        }

        public string RefreshToken(User user, string refreshToken)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Validates the JWT configuration settings.
        /// Throws exceptions if any required settings are missing or invalid.
        /// </summary>
        /// <param name="jwtConfiguration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void JwtTokenServiceValidator(IOptions<JwtConfiguration> jwtConfiguration)
        {
            if (jwtConfiguration == null)
            {
                throw new ArgumentNullException("JwtConfiguration cannot be null");
            }
            if (jwtConfiguration.Value == null)
            {
                throw new ArgumentNullException("JwtConfiguration.Value cannot be null");
            }

            if (string.IsNullOrEmpty(jwtConfiguration.Value.Issuer))
            {
                throw new ArgumentException("JwtConfiguration.Issuer cannot be null or empty");
            }

            if (string.IsNullOrEmpty(jwtConfiguration.Value.Audience))
            {
                throw new ArgumentException("JwtConfiguration.Audience cannot be null or empty");
            }

            if (string.IsNullOrEmpty(jwtConfiguration.Value.SecretKey))
            {
                throw new ArgumentException("JwtConfiguration.SecretKey cannot be null or empty");
            }

            if (jwtConfiguration.Value.ExpirationTime <= TimeSpan.Zero)
            {
                throw new ArgumentException("JwtConfiguration.ExpirationTime must be greater than zero");
            }
        }
    }
}