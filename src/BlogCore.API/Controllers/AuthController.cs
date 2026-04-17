using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>Registration result</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.DisplayName,
                Roles = request.Roles
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Access token and refresh token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var command = new LoginCommand
            {
                UsernameOrEmail = request.UsernameOrEmail,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Refreshes an expired access token using a refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Logs out a user by invalidating their refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Logout result</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto request)
        {
            var command = new LogoutCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Changes the password for the authenticated user
        /// </summary>
        /// <param name="request">Current and new password</param>
        /// <returns>Password change result</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userIdClaim = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(BaseResponse<object>.FailureResponse("User ID not found in token"));
            }

            var command = new ChangePasswordCommand
            {
                UserId = userId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true)
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the current authenticated user's information
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            // Multiple ways to get the user ID
            var userId = User.FindFirstValue("userId") ??           // Custom claim
                        User.FindFirstValue(ClaimTypes.NameIdentifier) ??  // Standard claim
                        User.FindFirstValue(JwtRegisteredClaimNames.Sub);   // JWT claim

            // Get username from various possible claim types
            var username = User.FindFirstValue(ClaimTypes.Name) ??           // "name"
                           User.FindFirstValue("username") ??                // "username"
                           User.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? // "unique_name"
                           User.FindFirstValue(JwtRegisteredClaimNames.Name);         // "name"

            // Get email
            var email = User.FindFirstValue(ClaimTypes.Email) ??
                        User.FindFirstValue(JwtRegisteredClaimNames.Email);

            var displayName = User.FindFirstValue("displayName");

            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var response = new UserInfoResponse
            {
                UserId = Guid.TryParse(userId, out Guid guid) ? guid : Guid.Empty,
                Username = username ?? string.Empty,
                Email = email ?? string.Empty,
                DisplayName = displayName ?? username ?? string.Empty,
                Roles = roles
            };

            return Ok(response);
        }
    }


}
