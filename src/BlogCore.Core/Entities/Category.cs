using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class Category : IEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        [JsonIgnore]
        public virtual Category? ParentCategory { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();

        [JsonIgnore]
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new HashSet<BlogPost>();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid Id { get; set; }

        public Category()
        {
            BlogPosts = new HashSet<BlogPost>();
            SubCategories = new HashSet<Category>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
