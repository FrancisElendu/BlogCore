namespace BlogCore.Application.DTOs.Auth
{
    /// <summary>
    /// User information response DTO
    /// </summary>
    public class UserInfoResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
