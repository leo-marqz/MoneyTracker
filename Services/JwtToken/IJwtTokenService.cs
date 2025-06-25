
using System;
using MoneyTracker.Models;

namespace MoneyTracker.Services.JwtToken
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        Guid GenerateRefreshToken();
        string RefreshToken(User user, string refreshToken);
    }
}