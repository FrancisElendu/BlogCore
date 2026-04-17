using BlogCore.Application.DTOs.Auth;
using System.Security.Claims;

namespace BlogCore.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        // Claim Management
        Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
        Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
        Task<IList<Claim>> GetUserClaimsAsync(Guid userId);

        // Role Management
        Task<bool> AddRoleToUserAsync(Guid userId, string role);
        Task<bool> RemoveRoleFromUserAsync(Guid userId, string role);
        Task<IList<string>> GetUserRolesAsync(Guid userId);

        // Sync Claims
        Task<bool> UpdateUserClaimsBasedOnRolesAsync(Guid userId);

        // Add this method if you need to get username
        Task<string> GetUsernameAsync(Guid userId);

        // Batch operations - NEW
        Task<BatchOperationResult> AddMultipleRolesToUserAsync(Guid userId, List<string> roles);
        Task<BatchOperationResult> RemoveMultipleRolesFromUserAsync(Guid userId, List<string> roles);
        Task<BatchOperationResult> AddMultipleClaimsToUserAsync(Guid userId, List<ClaimDto> claims);
        Task<BatchOperationResult> RemoveMultipleClaimsFromUserAsync(Guid userId, List<ClaimDto> claims);
    }
}
