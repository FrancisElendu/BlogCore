using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Auth
{
    #region Claim Management DTOs
    public class AddClaimRequestDto
    {
        [Required(ErrorMessage = "Claim type is required")]
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Claim value is required")]
        public string ClaimValue { get; set; }
    }
    public class RemoveClaimRequestDto
    {
        [Required(ErrorMessage = "Claim type is required")]
        public string ClaimType { get; set; }

        [Required(ErrorMessage = "Claim value is required")]
        public string ClaimValue { get; set; }
    }
    public class ClaimResponseDto
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
    #endregion

    #region Role Management DTOs
    public class AddRoleRequestDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Role { get; set; }
    }
    public class RemoveRoleRequestDto
    {
        [Required(ErrorMessage = "Role name is required")]
        public string Role { get; set; }
    }
    public class RoleResponseDto
    {
        public string Name { get; set; }
        public int UserCount { get; set; }
    }
    #endregion

    #region User Management Response DTOs
    public class UserClaimsResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public List<ClaimResponseDto> Claims { get; set; } = new();
    }
    public class UserRolesResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; } = new();
    }
    public class UserManagementResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }
    #endregion
}
