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

        public virtual ICollection<BlogPost> BlogPosts { get; set; }
        public Guid Id { get; set; }

        public User()
        {
            BlogPosts = new HashSet<BlogPost>();
        }
    }
}
