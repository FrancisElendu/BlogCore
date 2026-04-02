namespace BlogCore.Application.DTOs.Tag
{
    public class TagResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        // Statistics
        public int BlogPostCount { get; set; }

        // Recent posts using this tag (optional - for detail view)
        public List<TagBlogPostSummaryDto> RecentPosts { get; set; }

        // Audit information
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TagResponseDto()
        {
            RecentPosts = new List<TagBlogPostSummaryDto>();
        }
    }
}
