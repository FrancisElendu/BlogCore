namespace BlogCore.Application.DTOs.BlogPost
{
    public class BlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
    }
}
