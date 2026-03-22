using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.Core.Entities
{
    public class Category : IEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(200)]
        public string Slug { get; set; }

        public Guid? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; }
        public virtual ICollection<BlogPost> BlogPosts { get; set; }

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
