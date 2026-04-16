using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, BaseResponse<UserManagementResponseDto>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<RemoveRoleFromUserCommandHandler> _logger;

        public RemoveRoleFromUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<RemoveRoleFromUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }
        public async Task<BaseResponse<UserManagementResponseDto>> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userManagementService.RemoveRoleFromUserAsync(request.UserId, request.Role);

                if (!result)
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = $"Failed to remove role '{request.Role}' from user"
                    };
                }

                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = true,
                    Message = "Role removed successfully",
                    Data = new UserManagementResponseDto
                    {
                        Success = true,
                        Message = "Role removed successfully"
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
                _logger.LogError(ex, "Error removing role from user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
