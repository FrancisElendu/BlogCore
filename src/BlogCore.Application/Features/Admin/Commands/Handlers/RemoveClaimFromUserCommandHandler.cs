using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class RemoveClaimFromUserCommandHandler : IRequestHandler<RemoveClaimFromUserCommand, BaseResponse<UserManagementResponseDto>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<RemoveClaimFromUserCommandHandler> _logger;

        public RemoveClaimFromUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<RemoveClaimFromUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<UserManagementResponseDto>> Handle(RemoveClaimFromUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userManagementService.RemoveClaimFromUserAsync(request.UserId, request.ClaimType, request.ClaimValue);

                if (!result)
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = "Failed to remove claim from user"
                    };
                }

                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = true,
                    Message = "Claim removed successfully",
                    Data = new UserManagementResponseDto
                    {
                        Success = true,
                        Message = "Claim removed successfully"
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
                _logger.LogError(ex, "Error removing claim from user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
