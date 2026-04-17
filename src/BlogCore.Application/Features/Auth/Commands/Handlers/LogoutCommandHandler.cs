using BlogCore.Application.Common.Base;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Auth.Commands.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, BaseResponse<bool>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IAuthService authService,
            ILogger<LogoutCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        public async Task<BaseResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.LogoutAsync(request.RefreshToken);

                return BaseResponse<bool>.SuccessResponse(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                return BaseResponse<bool>.FailureResponse("An error occurred during logout");
            }
        }
    }
}
