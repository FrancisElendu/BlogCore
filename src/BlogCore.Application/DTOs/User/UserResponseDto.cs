namespace BlogCore.Application.DTOs.User
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;    
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int BlogPostCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }
}
