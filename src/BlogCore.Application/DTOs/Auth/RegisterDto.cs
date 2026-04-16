using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;   
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        // Optional: List of roles to assign to the user
        public List<string> Roles { get; set; } = new List<string>();
    }
}
