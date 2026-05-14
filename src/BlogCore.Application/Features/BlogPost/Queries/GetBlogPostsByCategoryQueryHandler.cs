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
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostsByCategoryQueryHandler : IRequestHandler<GetBlogPostsByCategoryQuery, BaseResponse<PagedResult<BlogPostSummaryDto>>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRepository<BlogCore.Core.Entities.Category> _sqlRepository;

        public GetBlogPostsByCategoryQueryHandler(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository, IRepository<BlogCore.Core.Entities.Category> sqlRepository)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _sqlRepository = sqlRepository;
        }

        public async Task<BaseResponse<PagedResult<BlogPostSummaryDto>>> Handle(GetBlogPostsByCategoryQuery request, CancellationToken cancellationToken)
        {
            // Validate pagination parameters
            ValidatePaginationParameters.PaginationParameters(request.PageNumber, request.PageSize);

            // Validate category exists
            //var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            var category = await _sqlRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), request.CategoryId);
            }

            var builder = new SpecificationBuilder<Core.Entities.BlogPost>()
                .Where(b => b.Categories.Any(c => c.Id == request.CategoryId))
                .Where(b => b.Status == PostStatus.Published); // Only show published posts in category view

            // Get total count
            var countSpec = builder.Build();
            var totalCount = await _blogPostRepository.CountAsync(countSpec, cancellationToken);

            if (totalCount == 0)
            {
                return BaseResponse<PagedResult<BlogPostSummaryDto>>.SuccessResponse(
                    EmptyPagedResult<BlogPostSummaryDto>.Create(request.PageNumber, request.PageSize),
                    $"No published blog posts found in category '{category.Name}'.");
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
