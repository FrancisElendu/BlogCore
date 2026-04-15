using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Admin.Commands
{
    public class AddRoleToUserCommand : IRequest<BaseResponse<UserManagementResponseDto>>
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
