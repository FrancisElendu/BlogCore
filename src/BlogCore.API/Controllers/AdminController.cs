using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.Auth;
using BlogCore.Application.Features.Admin.Commands;
using BlogCore.Application.Features.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IMediator mediator, ILogger<AdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Adds a claim to a user
        /// </summary>
        [HttpPost("users/{userId}/claims")]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddClaimToUser(Guid userId, [FromBody] AddClaimRequestDto request)
        {
            var command = new AddClaimToUserCommand
            {
                UserId = userId,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Removes a claim from a user
        /// </summary>
        [HttpDelete("users/{userId}/claims")]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveClaimFromUser(Guid userId, [FromBody] RemoveClaimRequestDto request)
        {
            var command = new RemoveClaimFromUserCommand
            {
                UserId = userId,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all claims for a user
        /// </summary>
        [HttpGet("users/{userId}/claims")]
        [ProducesResponseType(typeof(UserClaimsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserClaims(Guid userId)
        {
            var query = new GetUserClaimsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Adds a role to a user
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRoleToUser(Guid userId, [FromBody] AddRoleRequestDto request)
        {
            var command = new AddRoleToUserCommand
            {
                UserId = userId,
                Role = request.Role
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("not found"))
                    return NotFound(result);
                if (result.Message.Contains("already has role"))
                    return BadRequest(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Removes a role from a user
        /// </summary>
        [HttpDelete("users/{userId}/roles")]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, [FromBody] RemoveRoleRequestDto request)
        {
            var command = new RemoveRoleFromUserCommand
            {
                UserId = userId,
                Role = request.Role
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all roles for a user
        /// </summary>
        [HttpGet("users/{userId}/roles")]
        [ProducesResponseType(typeof(UserRolesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserRoles(Guid userId)
        {
            var query = new GetUserRolesQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Syncs a user's claims based on their current roles
        /// </summary>
        [HttpPost("users/{userId}/sync-claims")]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<UserManagementResponseDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SyncUserClaims(Guid userId)
        {
            var command = new SyncUserClaimsCommand { UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
