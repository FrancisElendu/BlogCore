using MSSQLFlexCrud;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.Core.Entities
{
    public class Tag : IEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Slug { get; set; }

        public virtual ICollection<BlogPost> BlogPosts { get; set; }
        public Guid Id { get; set; }

        public Tag()
        {
            BlogPosts = new HashSet<BlogPost>();
        }
    }
}
