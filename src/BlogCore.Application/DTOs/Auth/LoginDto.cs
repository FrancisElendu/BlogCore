using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
