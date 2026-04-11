using Microsoft.AspNetCore.Identity;
using MSSQLFlexCrud;
using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity
    {
        // Custom properties from your existing User class
        public string? DisplayName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; }

        // IEntity requires Id property
        // IdentityUser<Guid> already provides an Id property of type Guid
        // So we don't need to redeclare it - it's inherited

        public ApplicationUser()
        {
            BlogPosts = new HashSet<BlogPost>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;

            // Ensure Id is generated if not set
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
        }
    }
}
