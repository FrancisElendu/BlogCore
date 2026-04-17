using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Admin.Commands
{
    public class RemoveMultipleClaimsFromUserCommand : IRequest<BaseResponse<BatchOperationResult>>
    {
        public Guid UserId { get; set; }
        public List<ClaimDto> Claims { get; set; } = new();
    }
}
