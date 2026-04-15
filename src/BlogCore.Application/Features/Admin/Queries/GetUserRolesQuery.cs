using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Admin.Queries
{
    public class GetUserRolesQuery : IRequest<UserRolesResponseDto>
    {
        public Guid UserId { get; set; }
    }
}
