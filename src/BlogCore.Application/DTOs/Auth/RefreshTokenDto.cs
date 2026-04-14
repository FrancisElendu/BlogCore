using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
