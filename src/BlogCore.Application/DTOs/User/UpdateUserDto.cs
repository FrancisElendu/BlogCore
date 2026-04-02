using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(100, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens")]
        public string Username { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100)]
        public string DisplayName { get; set; }
    }
}
