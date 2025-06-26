
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Database;
using MoneyTracker.Models;

namespace MoneyTracker.Services.JwtToken
{
    public class JwtTokenService : IJwtTokenService
    {
        private ILogger<JwtTokenService> _logger;
        private MoneyTrackerDbContext _context;
        private JwtConfiguration _jwtConfiguration;

        public JwtTokenService(
            IOptions<JwtConfiguration> jwtConfiguration,
            ILogger<JwtTokenService> logger,
            MoneyTrackerDbContext context
            )
        {
            JwtTokenServiceValidator(jwtConfiguration);

            _jwtConfiguration = jwtConfiguration.Value;
            _logger = logger;
            _context = context;
        }

        public Guid GenerateRefreshToken()
        {
            return Guid.NewGuid();
        }

        public string GenerateToken(User user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfiguration.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Role", role), 
            };

            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(_jwtConfiguration.ExpirationTime),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

            if (jwtConfiguration.Value.SecretKey.Length < 32)
{
                throw new ArgumentException("JwtConfiguration.SecretKey must be at least 32 characters long");
            }

            if (jwtConfiguration.Value.ExpirationTime <= TimeSpan.Zero)
            {
                throw new ArgumentException("JwtConfiguration.ExpirationTime must be greater than zero");
            }
        }
    }
}