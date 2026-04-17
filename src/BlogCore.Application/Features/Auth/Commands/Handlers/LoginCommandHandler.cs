using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Auth.Commands.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<TokenResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IAuthService authService,
            ILogger<LoginCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<BaseResponse<TokenResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var loginDto = new LoginDto
                {
                    UsernameOrEmail = request.UsernameOrEmail,
                    Password = request.Password
                };

                var result = await _authService.LoginAsync(loginDto);

                return BaseResponse<TokenResponseDto>.SuccessResponse(result, "Login successful");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed for {UsernameOrEmail}", request.UsernameOrEmail);
                return BaseResponse<TokenResponseDto>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for {UsernameOrEmail}", request.UsernameOrEmail);
                return BaseResponse<TokenResponseDto>.FailureResponse("An error occurred during login");
            }
        }
    }
}
