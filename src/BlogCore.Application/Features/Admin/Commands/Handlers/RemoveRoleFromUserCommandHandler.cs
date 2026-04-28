using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
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
            _logger.LogInformation("Removing role '{Role}' from user {UserId}", request.Role, request.UserId);

            var result = await _userManagementService.RemoveRoleFromUserAsync(request.UserId, request.Role);

            if (!result)
            {
                throw new BusinessRuleException($"Failed to remove role '{request.Role}' from user");
            }

            _logger.LogInformation("Successfully removed role '{Role}' from user {UserId}", request.Role, request.UserId);

            return BaseResponse<UserManagementResponseDto>.SuccessResponse(
                new UserManagementResponseDto
                {
                    Success = true,
                    Message = "Role removed successfully"
                },
                "Role removed successfully");
        }
    }
}
