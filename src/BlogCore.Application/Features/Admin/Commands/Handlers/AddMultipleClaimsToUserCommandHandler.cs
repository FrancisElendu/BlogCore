using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class AddMultipleClaimsToUserCommandHandler : IRequestHandler<AddMultipleClaimsToUserCommand, BaseResponse<BatchOperationResult>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<AddMultipleClaimsToUserCommandHandler> _logger;

        public AddMultipleClaimsToUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<AddMultipleClaimsToUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<BatchOperationResult>> Handle(AddMultipleClaimsToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Claims == null || !request.Claims.Any())
                {
                    return BaseResponse<BatchOperationResult>.FailureResponse("No claims specified to add");
                }

                var result = await _userManagementService.AddMultipleClaimsToUserAsync(request.UserId, request.Claims);

                return BaseResponse<BatchOperationResult>.SuccessResponse(result, result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return BaseResponse<BatchOperationResult>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding multiple claims to user {UserId}", request.UserId);
                return BaseResponse<BatchOperationResult>.FailureResponse("An error occurred while adding claims");
            }
        }
    }
}
