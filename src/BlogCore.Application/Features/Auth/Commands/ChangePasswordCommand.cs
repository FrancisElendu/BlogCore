using BlogCore.Application.Common.Base;
using MediatR;

namespace BlogCore.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<BaseResponse<bool>>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
