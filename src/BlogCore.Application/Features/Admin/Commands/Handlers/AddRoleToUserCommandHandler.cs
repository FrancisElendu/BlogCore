using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
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
            _logger.LogInformation("Adding role '{Role}' to user {UserId}", request.Role, request.UserId);

            // First check if role exists (you may want to add this to UserManagementService)
            var roles = await _userManagementService.GetUserRolesAsync(request.UserId);

            if (roles.Contains(request.Role))
            {
                _logger.LogWarning("User {UserId} already has role '{Role}'", request.UserId, request.Role);
                throw new BusinessRuleException($"User already has role '{request.Role}'");
            }

            var result = await _userManagementService.AddRoleToUserAsync(request.UserId, request.Role);

            if (!result)
            {
                _logger.LogWarning("Failed to add role '{Role}' to user {UserId}", request.Role, request.UserId);
                throw new BusinessRuleException($"Failed to add role '{request.Role}' to user");
            }

            _logger.LogInformation("Successfully added role '{Role}' to user {UserId}", request.Role, request.UserId);

            return BaseResponse<UserManagementResponseDto>.SuccessResponse(
                new UserManagementResponseDto
                {
                    Success = true,
                    Message = "Role added successfully"
                },
                "Role added successfully");
        }
    }
}
