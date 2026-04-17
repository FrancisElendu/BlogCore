using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class AddMultipleRolesToUserCommandHandler : IRequestHandler<AddMultipleRolesToUserCommand, BaseResponse<BatchOperationResult>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<AddMultipleRolesToUserCommandHandler> _logger;

        public AddMultipleRolesToUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<AddMultipleRolesToUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<BatchOperationResult>> Handle(AddMultipleRolesToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Roles == null || !request.Roles.Any())
                {
                    return BaseResponse<BatchOperationResult>.FailureResponse("No roles specified to add");
                }

                var result = await _userManagementService.AddMultipleRolesToUserAsync(request.UserId, request.Roles);

                return BaseResponse<BatchOperationResult>.SuccessResponse(result, result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return BaseResponse<BatchOperationResult>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple roles to user {UserId}", request.UserId);
                return BaseResponse<BatchOperationResult>.FailureResponse("An error occurred while adding roles");
            }
        }
    }
}
