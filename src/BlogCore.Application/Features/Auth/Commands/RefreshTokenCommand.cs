using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<BaseResponse<TokenResponseDto>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
