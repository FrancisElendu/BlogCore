using BlogCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
