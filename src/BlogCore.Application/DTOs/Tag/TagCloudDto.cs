namespace BlogCore.Application.DTOs.Tag
{
    public class TagCloudDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int BlogPostCount { get; set; }

        // For tag cloud sizing (relative weight)
        public int Weight { get; set; }

        // CSS class for styling based on weight
        public string SizeClass { get; set; } = string.Empty;
    }
}
