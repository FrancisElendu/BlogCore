using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands
{
    public class AddClaimToUserCommand : IRequest<BaseResponse<UserManagementResponseDto>>
    {
        public Guid UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
    
}
