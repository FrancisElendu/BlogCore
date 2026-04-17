using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Admin.Commands.Handlers
{
    public class RemoveMultipleClaimsFromUserCommandHandler : IRequestHandler<RemoveMultipleClaimsFromUserCommand, BaseResponse<BatchOperationResult>>
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<RemoveMultipleClaimsFromUserCommandHandler> _logger;

        public RemoveMultipleClaimsFromUserCommandHandler(
            IUserManagementService userManagementService,
            ILogger<RemoveMultipleClaimsFromUserCommandHandler> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        public async Task<BaseResponse<BatchOperationResult>> Handle(RemoveMultipleClaimsFromUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Claims == null || !request.Claims.Any())
                {
                    return BaseResponse<BatchOperationResult>.FailureResponse("No claims specified to remove");
                }

                var result = await _userManagementService.RemoveMultipleClaimsFromUserAsync(request.UserId, request.Claims);

                return BaseResponse<BatchOperationResult>.SuccessResponse(result, result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return BaseResponse<BatchOperationResult>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing multiple claims from user {UserId}", request.UserId);
                return BaseResponse<BatchOperationResult>.FailureResponse("An error occurred while removing claims");
            }
        }
    }
}
