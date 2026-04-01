using System.ComponentModel.DataAnnotations;

namespace BlogCore.Core.DTOs
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
