using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Auth.Commands.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<TokenResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IAuthService authService,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        public async Task<BaseResponse<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.RefreshToken);

                return BaseResponse<TokenResponseDto>.SuccessResponse(result, "Token refreshed successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Token refresh failed");
                return BaseResponse<TokenResponseDto>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error");
                return BaseResponse<TokenResponseDto>.FailureResponse("An error occurred while refreshing token");
            }
        }
    }
}
