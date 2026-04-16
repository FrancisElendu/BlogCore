using BlogCore.Application.DTOs.BlogPost;

namespace BlogCore.Application.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public IEnumerable<BlogPostSummaryDto> RecentPosts { get; set; } = new List<BlogPostSummaryDto>();  
    }
}
