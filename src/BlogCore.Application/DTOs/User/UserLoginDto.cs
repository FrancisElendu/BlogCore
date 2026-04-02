using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.User
{
    public class UserLoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
