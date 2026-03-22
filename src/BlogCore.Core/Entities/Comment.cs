using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogCore.Core.Entities
{
    public class Comment : IEntity
    {
        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }

        public Guid BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost BlogPost { get; set; }

        public Guid? ParentCommentId { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; }

        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid Id { get; set; }

        public Comment()
        {
            Replies = new HashSet<Comment>();
            CreatedAt = DateTime.UtcNow;
            IsApproved = false;
            Id = Guid.NewGuid();
        }
    }
}
