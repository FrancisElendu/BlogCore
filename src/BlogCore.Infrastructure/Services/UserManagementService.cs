using BlogCore.Application.Interfaces.Services;
using BlogCore.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace BlogCore.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            ILogger<UserManagementService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Added claim {ClaimType}:{ClaimValue} to user {Username}",
                        claimType, claimValue, user.UserName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add claim to user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.RemoveClaimAsync(user, claim);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Removed claim {ClaimType}:{ClaimValue} from user {Username}",
                        claimType, claimValue, user.UserName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove claim from user {UserId}", userId);
                throw;
            }
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<bool> AddRoleToUserAsync(Guid userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                var result = await _userManager.AddToRoleAsync(user, role);

                if (result.Succeeded)
                {
                    // Update claims based on new role
                    await UpdateUserClaimsBasedOnRolesAsync(userId);
                    _logger.LogInformation("Added role {Role} to user {Username}", role, user.UserName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add role to user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                var result = await _userManager.RemoveFromRoleAsync(user, role);

                if (result.Succeeded)
                {
                    // Update claims based on removed role
                    await UpdateUserClaimsBasedOnRolesAsync(userId);
                    _logger.LogInformation("Removed role {Role} from user {Username}", role, user.UserName);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove role from user {UserId}", userId);
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> UpdateUserClaimsBasedOnRolesAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    throw new KeyNotFoundException($"User with ID {userId} not found");

                // Get current roles
                var roles = await _userManager.GetRolesAsync(user);

                // Remove all existing permission claims
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();

                foreach (var claim in permissionClaims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                // Add new claims based on current roles
                var newClaims = new List<Claim>();

                foreach (var role in roles)
                {
                    switch (role)
                    {
                        case "Admin":
                            newClaims.AddRange(GetAdminClaims());
                            break;
                        case "Author":
                            newClaims.AddRange(GetAuthorClaims());
                            break;
                        case "Editor":
                            newClaims.AddRange(GetEditorClaims());
                            break;
                        case "User":
                            newClaims.AddRange(GetUserClaims());
                            break;
                    }
                }

                // Add role claim for policy-based authorization
                foreach (var role in roles)
                {
                    newClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                foreach (var claim in newClaims.Distinct())
                {
                    await _userManager.AddClaimAsync(user, claim);
                }

                _logger.LogInformation("Updated claims for user {Username} based on roles: {Roles}",
                    user.UserName, string.Join(", ", roles));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update claims for user {UserId}", userId);
                throw;
            }
        }



        #region Private Helper Methods

        private IEnumerable<Claim> GetAdminClaims()
        {
            return new List<Claim>
            {
                new Claim("Permission", "users.view"),
                new Claim("Permission", "users.create"),
                new Claim("Permission", "users.edit"),
                new Claim("Permission", "users.delete"),
                new Claim("Permission", "posts.view.all"),
                new Claim("Permission", "posts.create"),
                new Claim("Permission", "posts.edit.all"),
                new Claim("Permission", "posts.delete.all"),
                new Claim("Permission", "posts.edit.own"),
                new Claim("Permission", "comments.moderate")
            };
        }

        private IEnumerable<Claim> GetAuthorClaims()
        {
            return new List<Claim>
            {
                new Claim("Permission", "posts.view.own"),
                new Claim("Permission", "posts.create"),
                new Claim("Permission", "posts.edit.own"),
                new Claim("Permission", "comments.view")
            };
        }

        private IEnumerable<Claim> GetEditorClaims()
        {
            return new List<Claim>
            {
                new Claim("Permission", "posts.view.all"),
                new Claim("Permission", "posts.edit.all"),
                new Claim("Permission", "comments.moderate")
            };
        }

        private IEnumerable<Claim> GetUserClaims()
        {
            return new List<Claim>
            {
                new Claim("Permission", "posts.view.published"),
                new Claim("Permission", "comments.create")
            };
        }

        #endregion
    }
}
