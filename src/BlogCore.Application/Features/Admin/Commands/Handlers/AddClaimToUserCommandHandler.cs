using BlogCore.Application.Common.Base;
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
            try
            {
                var result = await _userManagementService.AddClaimToUserAsync(request.UserId, request.ClaimType, request.ClaimValue);

                if (!result)
                {
                    return new BaseResponse<UserManagementResponseDto>
                    {
                        Success = false,
                        Message = "Failed to add claim to user"
                    };
                }

                return new BaseResponse<UserManagementResponseDto>
                {
                    Success = true,
                    Message = "Claim added successfully",
                    Data = new UserManagementResponseDto
                    {
                        Success = true,
                        Message = "Claim added successfully"
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
                _logger.LogError(ex, "Error adding claim to user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
