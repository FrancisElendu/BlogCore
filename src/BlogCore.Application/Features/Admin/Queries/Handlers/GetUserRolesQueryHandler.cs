using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Queries.Handlers
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, UserRolesResponseDto>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<GetUserRolesQueryHandler> _logger;

        public GetUserRolesQueryHandler(
            IUserManagementService userManagementService,
            ILogger<GetUserRolesQueryHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }
        public async Task<UserRolesResponseDto> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _userManagementService.GetUserRolesAsync(request.UserId);
                var username = await _userManagementService.GetUsernameAsync(request.UserId);

                return new UserRolesResponseDto
                {
                    UserId = request.UserId,
                    Username = username, // You might want to add a GetUsernameAsync method to UserManagementService
                    Roles = roles.ToList()
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User not found: {UserId}", request.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
