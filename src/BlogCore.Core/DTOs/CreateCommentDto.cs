using System.ComponentModel.DataAnnotations;

namespace BlogCore.Core.DTOs
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Comment content is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 500 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string AuthorEmail { get; set; }

        public Guid? ParentCommentId { get; set; }
    }
}
