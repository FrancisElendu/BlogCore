using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Admin.Commands
{
    public class RemoveMultipleRolesFromUserCommand : IRequest<BaseResponse<BatchOperationResult>>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
