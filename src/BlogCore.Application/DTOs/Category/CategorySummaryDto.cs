namespace BlogCore.Application.DTOs.Category
{
    public class CategorySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int BlogPostCount { get; set; }
    }
}
