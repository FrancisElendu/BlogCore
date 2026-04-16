using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class User : IEntity
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;  // Add this - stores hashed password
        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();
        public Guid Id { get; set; }

        // Optional: Add audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryDate { get; set; }

        public User()
        {
            BlogPosts = new HashSet<BlogPost>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
