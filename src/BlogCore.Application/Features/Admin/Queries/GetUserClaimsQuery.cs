using BlogCore.Application.DTOs.Auth;
using MediatR;

namespace BlogCore.Application.Features.Admin.Queries
{
    public class GetUserClaimsQuery : IRequest<UserClaimsResponseDto>
    {
        public Guid UserId { get; set; }
    }
}
