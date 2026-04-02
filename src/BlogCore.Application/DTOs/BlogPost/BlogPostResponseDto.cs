using BlogCore.Application.DTOs.Category;
using BlogCore.Application.DTOs.Comment;
using BlogCore.Application.DTOs.Tag;
using BlogCore.Core.Enums;

namespace BlogCore.Application.DTOs.BlogPost
{
    public class BlogPostResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public string FeaturedImageUrl { get; set; }
        public PostStatus Status { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<TagDto> Tags { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CommentCount { get; set; }  // Add this
        public List<CommentResponseDto> RecentComments { get; set; } // Optional: for detailed view
    }
}
