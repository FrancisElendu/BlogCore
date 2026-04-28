using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class SyncUserClaimsCommandHandler : IRequestHandler<SyncUserClaimsCommand, BaseResponse<UserManagementResponseDto>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<SyncUserClaimsCommandHandler> _logger;

        public SyncUserClaimsCommandHandler(
            IUserManagementService userManagementService,
            ILogger<SyncUserClaimsCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }
        public async Task<BaseResponse<UserManagementResponseDto>> Handle(SyncUserClaimsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting claim sync for user {UserId}", request.UserId);

            var result = await _userManagementService.UpdateUserClaimsBasedOnRolesAsync(request.UserId);

            if (!result)
            {
                _logger.LogWarning("Claim sync failed for user {UserId}", request.UserId);
                throw new BusinessRuleException($"Unable to sync claims for user {request.UserId}");
            }

            _logger.LogInformation("Claims synced successfully for user {UserId}", request.UserId);

            return BaseResponse<UserManagementResponseDto>.SuccessResponse(
                new UserManagementResponseDto
                {
                    Success = true,
                    Message = "Claims synced successfully"
                },
                "Claims synced successfully");
        }
    }
}
