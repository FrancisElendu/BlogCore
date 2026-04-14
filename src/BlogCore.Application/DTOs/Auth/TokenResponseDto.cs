namespace BlogCore.Application.DTOs.Auth
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
