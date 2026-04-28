using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.BlogPost;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostByIdQuery : IRequest<BaseResponse<BlogPostResponseDto>>
    {
        public Guid Id { get; set; }
    }
}
