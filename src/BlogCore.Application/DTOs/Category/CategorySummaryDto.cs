namespace BlogCore.Application.DTOs.Category
{
    public class CategorySummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int BlogPostCount { get; set; }
    }
}
