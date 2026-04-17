using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<BaseResponse<RegisterResponseDto>>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
