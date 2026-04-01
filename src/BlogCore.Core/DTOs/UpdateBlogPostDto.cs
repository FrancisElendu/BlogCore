using BlogCore.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.Core.DTOs
{
    public class UpdateBlogPostDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; }

        [MinLength(50, ErrorMessage = "Content must be at least 50 characters")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters")]
        public string Excerpt { get; set; }

        [StringLength(500, ErrorMessage = "Featured image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string FeaturedImageUrl { get; set; }

        public PostStatus? Status { get; set; }

        public List<Guid> CategoryIds { get; set; }
        public List<Guid> TagIds { get; set; }
    }
}
