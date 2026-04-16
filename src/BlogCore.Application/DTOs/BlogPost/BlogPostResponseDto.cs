using BlogCore.Application.DTOs.Category;
using BlogCore.Application.DTOs.Comment;
using BlogCore.Application.DTOs.Tag;
using BlogCore.Core.Enums;

namespace BlogCore.Application.DTOs.BlogPost
{
    public class BlogPostResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string FeaturedImageUrl { get; set; } = string.Empty;
        public PostStatus Status { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CommentCount { get; set; }  // Add this
        public List<CommentResponseDto> RecentComments { get; set; } = new List<CommentResponseDto>(); // Optional: for detailed view
    }
}
