using BlogCore.Application.DTOs.Auth;

namespace BlogCore.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> RevokeAllUserTokensAsync(Guid userId);
    }
}
