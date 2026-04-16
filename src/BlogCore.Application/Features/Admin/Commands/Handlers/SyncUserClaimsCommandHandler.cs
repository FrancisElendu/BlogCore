using BlogCore.Application.Common.Base;
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
            try
            {
                var result = await _userManagementService.UpdateUserClaimsBasedOnRolesAsync(request.UserId);

                if (!result)
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = "Failed to sync claims for user"
                    };
                }

                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = true,
                    Message = "Claims synced successfully",
                    Data = new UserManagementResponseDto
                    {
                        Success = true,
                        Message = "Claims synced successfully"
                    }
                };
            }
            catch (KeyNotFoundException ex)
            {
                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing claims for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
