using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Common.Mappings;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Features.BlogPost.Validations;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostsByTagQueryHandler : IRequestHandler<GetBlogPostsByTagQuery, BaseResponse<PagedResult<BlogPostSummaryDto>>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ITagRepository _tagRepository;

        public GetBlogPostsByTagQueryHandler(
            IBlogPostRepository blogPostRepository,
            ITagRepository tagRepository)
        {
            _blogPostRepository = blogPostRepository;
            _tagRepository = tagRepository;
        }

        public async Task<BaseResponse<PagedResult<BlogPostSummaryDto>>> Handle(GetBlogPostsByTagQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            ValidatePaginationParameters.PaginationParameters(request.PageNumber, request.PageSize);

            // Validate tag exists
            var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
            if (tag == null)
            {
                throw new NotFoundException(nameof(Tag), request.TagId);
            }

            var builder = new SpecificationBuilder<Core.Entities.BlogPost>()
                .Where(b => b.Tags.Any(t => t.Id == request.TagId))
                .Where(b => b.Status == PostStatus.Published); // Only show published posts in tag view

            // Get total count
            var countSpec = builder.Build();
            var totalCount = await _blogPostRepository.CountAsync(countSpec, cancellationToken);

            if (totalCount == 0)
            {
                return BaseResponse<PagedResult<BlogPostSummaryDto>>.SuccessResponse(
                    EmptyPagedResult<BlogPostSummaryDto>.Create(request.PageNumber, request.PageSize),
                    $"No published blog posts found with tag '{tag.Name}'.");
            }

            // Apply sorting and pagination
            var dataSpec = builder
                .OrderByDescending(b => b.PublishedAt)
                .Page(request.PageNumber, request.PageSize)
                .Include(b => b.Categories)
                .Include(b => b.Tags)
                .Build();

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
