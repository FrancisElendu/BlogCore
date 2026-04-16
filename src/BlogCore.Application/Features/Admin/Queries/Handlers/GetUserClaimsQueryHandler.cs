using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Queries.Handlers
{
    public class GetUserClaimsQueryHandler : IRequestHandler<GetUserClaimsQuery, UserClaimsResponseDto>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<GetUserClaimsQueryHandler> _logger;

        public GetUserClaimsQueryHandler(
            IUserManagementService userManagementService,
            ILogger<GetUserClaimsQueryHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }
        public async Task<UserClaimsResponseDto> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var claims = await _userManagementService.GetUserClaimsAsync(request.UserId);

                var username = await _userManagementService.GetUsernameAsync(request.UserId);

                // You might want to get the username separately
                // For now, we'll get it from the claims or you can add a method to UserManagementService
                //var username = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value ?? "Unknown";

                return new UserClaimsResponseDto
                {
                    UserId = request.UserId,
                    Username = username,
                    Claims = claims.Select(c => new ClaimResponseDto
                    {
                        Type = c.Type,
                        Value = c.Value
                    }).ToList()
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User not found: {UserId}", request.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting claims for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
