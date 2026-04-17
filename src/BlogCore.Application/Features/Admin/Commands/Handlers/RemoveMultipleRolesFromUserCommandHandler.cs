using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class RemoveMultipleRolesFromUserCommandHandler : IRequestHandler<RemoveMultipleRolesFromUserCommand, BaseResponse<BatchOperationResult>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<RemoveMultipleRolesFromUserCommandHandler> _logger;

        public RemoveMultipleRolesFromUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<RemoveMultipleRolesFromUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<BatchOperationResult>> Handle(RemoveMultipleRolesFromUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Roles == null || !request.Roles.Any())
                {
                    return BaseResponse<BatchOperationResult>.FailureResponse("No roles specified to remove");
                }

                var result = await _userManagementService.RemoveMultipleRolesFromUserAsync(request.UserId, request.Roles);

                return BaseResponse<BatchOperationResult>.SuccessResponse(result, result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return BaseResponse<BatchOperationResult>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing multiple roles from user {UserId}", request.UserId);
                return BaseResponse<BatchOperationResult>.FailureResponse("An error occurred while removing roles");
            }
        }
    }
}
