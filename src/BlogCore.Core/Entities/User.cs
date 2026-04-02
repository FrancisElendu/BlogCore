using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.Core.Entities
{
    public class User : IEntity
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Add this - stores hashed password

        public virtual ICollection<BlogPost> BlogPosts { get; set; }
        public Guid Id { get; set; }

        // Optional: Add audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            BlogPosts = new HashSet<BlogPost>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
