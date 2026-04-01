namespace BlogCore.Core.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<BlogPostSummaryDto> RecentPosts { get; set; }
    }
}
