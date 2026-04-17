using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Auth
{
    #region Claim Management DTOs
    public class AddClaimRequestDto
    {
        [Required(ErrorMessage = "Claim type is required")]
        public string ClaimType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Claim value is required")]
        public string ClaimValue { get; set; } = string.Empty;
    }
    public class RemoveClaimRequestDto
    {
        [Required(ErrorMessage = "Claim type is required")]
        public string ClaimType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Claim value is required")]
        public string ClaimValue { get; set; } = string.Empty;  
    }
    public class ClaimResponseDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
    #endregion

    #region Role Management DTOs
    public class AddRoleRequestDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Role { get; set; } = string.Empty;
    }
    public class RemoveRoleRequestDto
    {
        [Required(ErrorMessage = "Role name is required")]
        public string Role { get; set; } = string.Empty;
    }
    public class RoleResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
    #endregion

    #region User Management Response DTOs
    public class UserClaimsResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<ClaimResponseDto> Claims { get; set; } = new();
    }
    public class UserRolesResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
    public class UserManagementResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
    #endregion

    public class ClaimDto
    {
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }

    public class BatchOperationResult
    {
        public bool Success { get; set; }
        public List<string> SuccessfulItems { get; set; } = new();
        public List<string> FailedItems { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    #region Batch Request DTOs

    public class AddMultipleRolesRequest
    {
        public List<string> Roles { get; set; } = new();
    }

    public class RemoveMultipleRolesRequest
    {
        public List<string> Roles { get; set; } = new();
    }

    public class AddMultipleClaimsRequest
    {
        public List<ClaimDto> Claims { get; set; } = new();
    }

    public class RemoveMultipleClaimsRequest
    {
        public List<ClaimDto> Claims { get; set; } = new();
    }

    #endregion
}
