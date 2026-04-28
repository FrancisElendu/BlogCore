using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class AddClaimToUserCommandHandler : IRequestHandler<AddClaimToUserCommand, BaseResponse<UserManagementResponseDto>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<AddClaimToUserCommandHandler> _logger;

        public AddClaimToUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<AddClaimToUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<UserManagementResponseDto>> Handle(AddClaimToUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding claim '{ClaimType}':'{ClaimValue}' to user {UserId}",
                request.ClaimType, request.ClaimValue, request.UserId);

            var result = await _userManagementService.AddClaimToUserAsync(request.UserId, request.ClaimType, request.ClaimValue);

            if (!result)
            {
                _logger.LogWarning("Failed to add claim '{ClaimType}':'{ClaimValue}' to user {UserId}",
                    request.ClaimType, request.ClaimValue, request.UserId);
                throw new BusinessRuleException($"Failed to add claim '{request.ClaimType}' to user");
            }

            _logger.LogInformation("Successfully added claim '{ClaimType}':'{ClaimValue}' to user {UserId}",
                request.ClaimType, request.ClaimValue, request.UserId);

            return BaseResponse<UserManagementResponseDto>.SuccessResponse(
                new UserManagementResponseDto
                {
                    Success = true,
                    Message = "Claim added successfully"
                },
                "Claim added successfully");
        }
    }
}
