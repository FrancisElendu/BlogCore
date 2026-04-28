using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.BlogPost;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostsByCategoryQuery : IRequest<BaseResponse<PagedResult<BlogPostSummaryDto>>>
    {
        public Guid CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
