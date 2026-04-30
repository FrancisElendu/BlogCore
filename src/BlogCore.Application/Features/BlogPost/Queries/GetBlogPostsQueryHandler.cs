using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Mappings;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Features.BlogPost.Queries.Filters;
using BlogCore.Application.Features.BlogPost.Validations;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostsQueryHandler : IRequestHandler<GetBlogPostsQuery, BaseResponse<PagedResult<BlogPostSummaryDto>>>
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public GetBlogPostsQueryHandler(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<BaseResponse<PagedResult<BlogPostSummaryDto>>> Handle(GetBlogPostsQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            ValidatePaginationParameters.PaginationParameters(request.PageNumber, request.PageSize);

            var builder = new SpecificationBuilder<Core.Entities.BlogPost>();

            // Apply filters
            BaseFilter.ApplySearchFilter(builder, request.SearchTerm);
            BaseFilter.ApplyStatusFilter(builder, request.Status);
            BaseFilter.ApplyAuthorFilter(builder, request.AuthorId);
            BaseFilter.ApplyCategoryFilter(builder, request.CategoryId);
            BaseFilter.ApplyTagFilter(builder, request.TagId);
            BaseFilter.ApplyDateRangeFilter(builder, request.FromDate, request.ToDate);

            // Get total count for pagination
            var countSpec = builder.Build();
            var totalCount = await _blogPostRepository.CountAsync(countSpec, cancellationToken);

            // If no results, return empty paged result
            if (totalCount == 0)
            {
                return BaseResponse<PagedResult<BlogPostSummaryDto>>.SuccessResponse(
                    EmptyPagedResult<BlogPostSummaryDto>.Create(request.PageNumber, request.PageSize),
                    "No blog posts found matching the criteria.");
            }

            // Apply sorting
            BaseFilter.ApplySorting(builder, request.SortBy, request.SortDescending);

            // Apply pagination
            builder.Page(request.PageNumber, request.PageSize);

            // Include navigation properties
            builder.Include(b => b.Categories);
            builder.Include(b => b.Tags);

            var dataSpec = builder.Build();
            var blogPosts = await _blogPostRepository.FindAsync(dataSpec, cancellationToken);

            var result = new PagedResult<BlogPostSummaryDto>
            {
                Items = ManualMapper.MapToBlogPostSummaryDtoList(blogPosts),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return BaseResponse<PagedResult<BlogPostSummaryDto>>.SuccessResponse(result);
        }
    }
}
