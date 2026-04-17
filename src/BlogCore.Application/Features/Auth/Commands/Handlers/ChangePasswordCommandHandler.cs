using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Auth.Commands.Handlers
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<object>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            IAuthService authService,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        public async Task<BaseResponse<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var changePasswordDto = new ChangePasswordDto
                {
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword
                };

                var result = await _authService.ChangePasswordAsync(request.UserId, changePasswordDto);

                if (!result)
                {
                    return BaseResponse<object>.FailureResponse("Failed to change password");
                }

                return BaseResponse<object>.SuccessResponse(null, "Password changed successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return BaseResponse<object>.FailureResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BaseResponse<object>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change error for user {UserId}", request.UserId);
                return BaseResponse<object>.FailureResponse("An error occurred while changing password");
            }
        }
    }
}
