using BlogCore.Application.Common.Base;
using MediatR;

namespace BlogCore.Application.Features.Auth.Commands
{
    public class LogoutCommand : IRequest<BaseResponse<object>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
