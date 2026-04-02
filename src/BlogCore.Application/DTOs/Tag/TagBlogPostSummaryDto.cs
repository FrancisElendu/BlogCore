namespace BlogCore.Application.DTOs.Tag
{
    public class TagBlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
