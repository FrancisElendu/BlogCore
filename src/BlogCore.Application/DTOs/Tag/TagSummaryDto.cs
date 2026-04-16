namespace BlogCore.Application.DTOs.Tag
{
    public class TagSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int BlogPostCount { get; set; }
    }
}
