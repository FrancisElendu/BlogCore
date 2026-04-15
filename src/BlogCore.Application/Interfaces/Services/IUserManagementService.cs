using System.Security.Claims;

namespace BlogCore.Application.Interfaces.Services
{
    public interface IUserManagementService
    {
        //Task<bool> AddClaimToUserAsync(Guid userId, string claimType, string claimValue);
        //Task<bool> RemoveClaimFromUserAsync(Guid userId, string claimType, string claimValue);
        //Task<IList<Claim>> GetUserClaimsAsync(Guid userId);
        //Task<bool> AddRoleToUserAsync(Guid userId, string role);
        //Task<bool> RemoveRoleFromUserAsync(Guid userId, string role);
        //Task<IList<string>> GetUserRolesAsync(Guid userId);
        //Task<bool> UpdateUserClaimsBasedOnRolesAsync(Guid userId);

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
    }
}
