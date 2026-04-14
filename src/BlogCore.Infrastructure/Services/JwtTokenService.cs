using BlogCore.Application.Interfaces.Services;
using BlogCore.Application.JWT;
using BlogCore.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogCore.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

            // Load JWT settings
            var jwtSection = _configuration.GetSection("JwtSettings");
            _jwtSettings = new JwtSettings
            {
                Secret = jwtSection["Secret"],
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                AccessTokenExpiryMinutes = Convert.ToDouble(jwtSection["AccessTokenExpiryMinutes"]),
                RefreshTokenExpiryDays = Convert.ToInt32(jwtSection["RefreshTokenExpiryDays"])
            };
        }

        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Get user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("displayName", user.DisplayName ?? user.UserName)
            };

            // Add role claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add user claims
            claims.AddRange(userClaims);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Don't validate lifetime for expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetAccessTokenExpiryMinutes()
        {
            return (int)_jwtSettings.AccessTokenExpiryMinutes;
        }

        public int GetRefreshTokenExpiryDays()
        {
            return _jwtSettings.RefreshTokenExpiryDays;
        }
    }
}
