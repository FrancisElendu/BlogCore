using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, BaseResponse<UserManagementResponseDto>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<AddRoleToUserCommandHandler> _logger;

        public AddRoleToUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<AddRoleToUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<UserManagementResponseDto>> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // First check if role exists (you may want to add this to UserManagementService)
                var roles = await _userManagementService.GetUserRolesAsync(request.UserId);
                if (roles.Contains(request.Role))
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = $"User already has role '{request.Role}'"
                    };
                }

                var result = await _userManagementService.AddRoleToUserAsync(request.UserId, request.Role);

                if (!result)
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = $"Failed to add role '{request.Role}' to user"
                    };
                }

                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = true,
                    Message = "Role added successfully",
                    Data = new UserManagementResponseDto
                    {
                        Success = true,
                        Message = "Role added successfully"
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
                _logger.LogError(ex, "Error adding role to user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
