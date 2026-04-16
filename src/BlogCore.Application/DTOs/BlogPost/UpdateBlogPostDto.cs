using BlogCore.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.BlogPost
{
    public class UpdateBlogPostDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [MinLength(50, ErrorMessage = "Content must be at least 50 characters")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters")]
        public string Excerpt { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Featured image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string FeaturedImageUrl { get; set; } = string.Empty;
        public PostStatus? Status { get; set; }

        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
