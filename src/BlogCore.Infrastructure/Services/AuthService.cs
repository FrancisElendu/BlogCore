using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlogCore.Infrastructure.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly BlogDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            BlogDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by username or email
                var user = await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);
                if (user == null)
                    user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid credentials");

                if (!user.IsActive)
                    throw new UnauthorizedAccessException("Account is disabled");

                // Validate password
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                    throw new UnauthorizedAccessException("Invalid credentials");

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate tokens using the injected service
                var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtTokenService.GetRefreshTokenExpiryDays());

                // Save refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryDate = refreshTokenExpiry;
                await _userManager.UpdateAsync(user);

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("User logged in: {Username}", user.UserName);

                return new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtTokenService.GetAccessTokenExpiryMinutes()),
                    RefreshTokenExpiry = refreshTokenExpiry,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Username}", loginDto.UsernameOrEmail);
                throw;
            }
        }
        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
                if (existingUser != null)
                    throw new InvalidOperationException("Username already exists");

                existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("Email already exists");

                // Create new user
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    DisplayName = registerDto.DisplayName ?? registerDto.Username,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true // For demo, set true. In production, send confirmation email
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"User creation failed: {errors}");
                }

                // Assign roles
                await AssignRolesToUserAsync(user, registerDto.Roles);

                // Assign default claims based on roles
                await AssignDefaultClaimsToUserAsync(user, registerDto.Roles);


                _logger.LogInformation("User registered successfully: {Username}", user.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Username}", registerDto.Username);
                throw;
            }
        }
        public async Task<bool> RevokeAllUserTokensAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException("User not found");

                user.RefreshToken = null;
                user.RefreshTokenExpiryDate = null;
                user.SecurityStamp = Guid.NewGuid().ToString(); // Invalidates all existing tokens
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("All tokens revoked for user: {Username}", user.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token revocation failed for user {UserId}", userId);
                throw;
            }
        }
        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException("User not found");

                var result = await _userManager.ChangePasswordAsync(user,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Password change failed: {errors}");
                }

                // Revoke all refresh tokens after password change
                user.RefreshToken = null;
                user.RefreshTokenExpiryDate = null;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password changed for user: {Username}", user.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed for user {UserId}", userId);
                throw;
            }
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            try
            {
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryDate = null;
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("User logged out: {Username}", user.UserName);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                throw;
            }
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find user by refresh token
                var user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid refresh token");

                if (user.RefreshTokenExpiryDate < DateTime.UtcNow)
                    throw new UnauthorizedAccessException("Refresh token expired");

                if (!user.IsActive)
                    throw new UnauthorizedAccessException("Account is disabled");

                // Generate new tokens
                var newAccessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
                var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtTokenService.GetRefreshTokenExpiryDays());

                // Update refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryDate = newRefreshTokenExpiry;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("Token refreshed for user: {Username}", user.UserName);

                return new TokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtTokenService.GetAccessTokenExpiryMinutes()),
                    RefreshTokenExpiry = newRefreshTokenExpiry,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                throw;
            }
        }

        #region  Private Helper Methods for Role Assignment

        /// <summary>
        /// Assigns roles to a user with validation
        /// </summary>
        private async Task AssignRolesToUserAsync(ApplicationUser user, List<string> requestedRoles)
        {
            // Define allowed roles (you can also get this from configuration)
            //var allowedRoles = new HashSet<string>
            //{
            //    "Admin",
            //    "Author",
            //    "Editor",
            //    "User"
            //};

            // Read a string array
            var allowedRoles = _configuration.GetSection("Auth:AllowedRoles").Get<List<string>>();

            // If no roles specified, assign default "User" role
            if (requestedRoles == null || !requestedRoles.Any())
            {
                await _userManager.AddToRoleAsync(user, "User");
                return;
            }

            // Validate requested roles
            var invalidRoles = requestedRoles.Where(r => !allowedRoles?.Contains(r) ?? true).ToList();
            if (invalidRoles.Any())
            {
                throw new InvalidOperationException($"Invalid role(s) requested: {string.Join(", ", invalidRoles)}. " +
                    $"Allowed roles are: {string.Join(", ", allowedRoles ?? new List<string>())}");
            }

            // Check if the requesting user (if any) has permission to assign certain roles
            // This is important - you don't want regular users to assign Admin roles!
            var hasAdminRole = requestedRoles.Contains("Admin");
            if (hasAdminRole)
            {
                // You might want to check if the current user has permission to assign Admin role
                // For now, we'll only allow Admin role assignment in development or via a flag
                var allowAdminAssignment = _configuration.GetValue<bool>("Auth:AllowAdminRegistration", false);

                if (!allowAdminAssignment)
                {
                    _logger.LogWarning("Attempt to assign Admin role to user {Username} but Admin registration is disabled", user.UserName);
                    requestedRoles.Remove("Admin");

                    if (!requestedRoles.Any())
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        return;
                    }
                }
            }

            // Assign all valid roles
            foreach (var role in requestedRoles)
            {
                if (allowedRoles?.Contains(role) ?? false)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }


        /// <summary>
        /// Assigns default claims to a user based on their roles
        /// </summary>
        private async Task AssignDefaultClaimsToUserAsync(ApplicationUser user, List<string> roles)
        {
            var claims = new List<Claim>();

            // Base claims for all users
            claims.Add(new Claim("UserId", user.Id.ToString()));
            claims.Add(new Claim("Email", user.Email ?? string.Empty));
            claims.Add(new Claim("DisplayName", user.DisplayName ?? user.UserName ?? string.Empty));
            claims.Add(new Claim("AccountCreated", user.CreatedAt.ToString("o")));

            // Role-based claims
            if (roles.Contains("Admin"))
            {
                claims.Add(new Claim("Permission", "users.view"));
                claims.Add(new Claim("Permission", "users.create"));
                claims.Add(new Claim("Permission", "users.edit"));
                claims.Add(new Claim("Permission", "users.delete"));
                claims.Add(new Claim("Permission", "posts.view.all"));
                claims.Add(new Claim("Permission", "posts.create"));
                claims.Add(new Claim("Permission", "posts.edit.all"));
                claims.Add(new Claim("Permission", "posts.delete.all"));
                claims.Add(new Claim("Permission", "posts.edit.own"));
                claims.Add(new Claim("Permission", "comments.moderate"));
                claims.Add(new Claim("Permission", "posts.view.own")); // Also included
                claims.Add(new Claim("Permission", "posts.view.published")); // Also included

                // Comment moderation
                claims.Add(new Claim("Permission", "comments.moderate"));
                claims.Add(new Claim("Permission", "comments.view"));
                claims.Add(new Claim("Role", "Admin"));
            }
            else if (roles.Contains("Author"))
            {
                claims.Add(new Claim("Permission", "posts.view.own"));
                claims.Add(new Claim("Permission", "posts.create"));
                claims.Add(new Claim("Permission", "posts.edit.own"));
                claims.Add(new Claim("Permission", "comments.view"));
                claims.Add(new Claim("Role", "Author"));
            }
            else if (roles.Contains("Editor"))
            {
                claims.Add(new Claim("Permission", "posts.view.all"));
                claims.Add(new Claim("Permission", "posts.edit.all"));
                claims.Add(new Claim("Permission", "comments.moderate"));
                claims.Add(new Claim("Role", "Editor"));
            }
            else // Default User role
            {
                claims.Add(new Claim("Permission", "posts.view.published"));
                claims.Add(new Claim("Permission", "comments.create"));
                claims.Add(new Claim("Role", "User"));
            }

            // Add all claims to the user
            foreach (var claim in claims)
            {
                await _userManager.AddClaimAsync(user, claim);
            }

            _logger.LogInformation("Assigned {ClaimCount} claims to user {Username}", claims.Count, user.UserName);
        }
        #endregion

        //#region Private helper Methods for token and refresh tokens generation

        //private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        //{
        //    var jwtSettings = _configuration.GetSection("JwtSettings");
        //    var secret = jwtSettings["Secret"];
        //    var issuer = jwtSettings["Issuer"];
        //    var audience = jwtSettings["Audience"];
        //    var expiryMinutes = Convert.ToDouble(jwtSettings["AccessTokenExpiryMinutes"]);

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    // Get user claims
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    var userRoles = await _userManager.GetRolesAsync(user);

        //    var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        //        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim("userId", user.Id.ToString()),
        //        new Claim("displayName", user.DisplayName ?? user.UserName)
        //    };

        //    // Add role claims
        //    foreach (var role in userRoles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    // Add user claims
        //    claims.AddRange(userClaims);

        //    var token = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //private string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using var rng = RandomNumberGenerator.Create();
        //    rng.GetBytes(randomNumber);
        //    return Convert.ToBase64String(randomNumber);
        //}

        //#endregion

    }
}
