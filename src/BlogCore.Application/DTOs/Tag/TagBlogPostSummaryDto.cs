namespace BlogCore.Application.DTOs.Tag
{
    public class TagBlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
    }
}
