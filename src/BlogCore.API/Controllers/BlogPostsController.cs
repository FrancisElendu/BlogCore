using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Features.BlogPost.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BlogPostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(IMediator mediator, ILogger<BlogPostsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new blog post
        /// </summary>
        /// <param name="command">The blog post creation command</param>
        /// <returns>The created blog post</returns>
        /// <response code="201">Returns the created blog post</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If unauthorized</response>
        /// <response code="409">If a duplicate slug exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<BlogPostResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BaseResponse<BlogPostResponseDto>>> CreateBlogPost(
            [FromBody] CreateBlogPostCommand command,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new blog post with title: {Title}", command.Title);

            var response = await _mediator.Send(command, cancellationToken);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            // Return 201 Created with location header
            return CreatedAtAction(
                nameof(GetBlogPostById),
                new { id = response.Data?.Id },
                response);
        }

        /// <summary>
        /// Get a blog post by ID (placeholder for the CreatedAtAction)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<BlogPostResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BaseResponse<BlogPostResponseDto>>> GetBlogPostById(
            Guid id,
            CancellationToken cancellationToken)
        {
            // This is a placeholder - implement your get by id query here
            // For now, return not implemented
            return Ok(new BaseResponse<BlogPostResponseDto>("Get by ID not implemented yet"));
        }
    }
}
