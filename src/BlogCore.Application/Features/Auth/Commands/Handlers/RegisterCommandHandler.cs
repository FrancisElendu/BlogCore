using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlogCore.Application.Features.Auth.Commands.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, BaseResponse<RegisterResponseDto>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            IAuthService authService,
            ILogger<RegisterCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<BaseResponse<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var registerDto = new RegisterDto
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    DisplayName = request.DisplayName,
                    Roles = request.Roles
                };

                var result = await _authService.RegisterAsync(registerDto);

                if (!result)
                {
                    return BaseResponse<RegisterResponseDto>.FailureResponse("Registration failed");
                }

                var response = new RegisterResponseDto
                {
                    Username = request.Username,
                    Email = request.Email,
                    Roles = request.Roles,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("User registered successfully: {Username}", request.Username);

                return BaseResponse<RegisterResponseDto>.SuccessResponse(response, "User registered successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration validation failed for {Username}", request.Username);
                return BaseResponse<RegisterResponseDto>.FailureResponse(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Username}", request.Username);
                return BaseResponse<RegisterResponseDto>.FailureResponse("An error occurred during registration");
            }
        }
    }
}
