using BlogCore.Core.Enums;
using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class BlogPost : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string Excerpt { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; } = string.Empty;


        [StringLength(500)]
        public string FeaturedImageUrl { get; set; } = string.Empty;

        public PostStatus Status { get; set; } = PostStatus.Draft;

        public int ViewCount { get; set; }
        public int LikeCount { get; set; }

        public Guid AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        [JsonIgnore]
        public virtual ApplicationUser Author { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        [JsonIgnore]
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        

        public BlogPost()
        {
            Categories = new HashSet<Category>();
            //Comments = new HashSet<Comment>();
            Tags = new HashSet<Tag>();
            CreatedAt = DateTime.UtcNow;
            Status = PostStatus.Draft;
        }
    }
}

