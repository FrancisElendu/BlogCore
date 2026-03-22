using BlogCore.Core.Enums;
using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.Core.Entities
{
    public class BlogPost : IEntity
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Excerpt { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        [StringLength(500)]
        public string FeaturedImageUrl { get; set; }

        public PostStatus Status { get; set; }

        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        public Guid AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid Id { get; set; }

        public BlogPost()
        {
            Categories = new HashSet<Category>();
            Comments = new HashSet<Comment>();
            Tags = new HashSet<Tag>();
            CreatedAt = DateTime.UtcNow;
            Status = PostStatus.Draft;
        }
    }
}

