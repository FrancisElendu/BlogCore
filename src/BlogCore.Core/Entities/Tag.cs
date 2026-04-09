using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class Tag : IEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Slug { get; set; }

        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; }
        public Guid Id { get; set; }

        // Add audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Tag()
        {
            BlogPosts = new HashSet<BlogPost>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
