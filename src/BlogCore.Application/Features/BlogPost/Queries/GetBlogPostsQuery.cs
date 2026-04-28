using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Core.Enums;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostsQuery : IRequest<BaseResponse<PagedResult<BlogPostSummaryDto>>>
    {
        public string? SearchTerm { get; set; }
        public PostStatus? Status { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TagId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
