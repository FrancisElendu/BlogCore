using BlogCore.Core.Entities;
using System.Security.Claims;

namespace BlogCore.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
        int GetAccessTokenExpiryMinutes();
        int GetRefreshTokenExpiryDays();
    }
}
