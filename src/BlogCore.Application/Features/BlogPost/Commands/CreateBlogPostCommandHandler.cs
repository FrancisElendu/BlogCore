using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Common.Mappings;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;
using MediatR;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class CreateBlogPostCommandHandler : IRequestHandler<CreateBlogPostCommand, BaseResponse<BlogPostResponseDto>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IRepository<BlogCore.Core.Entities.BlogPost> _sqlRepository;

        public CreateBlogPostCommandHandler(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            IRepository<BlogCore.Core.Entities.BlogPost> sqlRepository
            )
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _sqlRepository = sqlRepository;
        }
        public async Task<BaseResponse<BlogPostResponseDto>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
        {
            // Generate slug from title
            var slug = BaseGenerateSlug.GenerateSlug(request.Title);

            // Check for duplicate slug
            var existingSlugSpec = new SpecificationBuilder<BlogCore.Core.Entities.BlogPost>()
                .Where(b => b.Slug == slug)
                .Build();

            var slugExists = await _blogPostRepository.AnyAsync(existingSlugSpec, cancellationToken);
            if (slugExists)
            {
                throw new DuplicateException(nameof(BlogPost), "Slug", slug);
            }

            // Validate author exists (you would typically call a User service)
            // This is a placeholder - implement actual user validation
            if (request.AuthorId == Guid.Empty)
            {
                throw new BusinessRuleException("AuthorId", "Author ID is required");
            }

            // Create blog post entity
            var blogPost = new Core.Entities.BlogPost
            {
                Title = request.Title,
                Slug = slug,
                Excerpt = request.Excerpt,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                Status = request.Status,
                AuthorId = request.AuthorId,
                CreatedAt = DateTime.UtcNow
            };

            // Set published date if status is Published
            if (request.Status == PostStatus.Published)
            {
                blogPost.PublishedAt = request.ScheduledPublishDate ?? DateTime.UtcNow;
            }

            // Add categories if provided
            if (request.CategoryIds.Any())
            {
                var categorySpec = new SpecificationBuilder<Category>()
                    .WhereIn(c => c.Id, request.CategoryIds)
                    .Build();

                var categories = await _categoryRepository.FindTrackedAsync(categorySpec, cancellationToken);

                if (categories.Count != request.CategoryIds.Count)
                {
                    var foundIds = categories.Select(c => c.Id).ToHashSet();
                    var missingIds = request.CategoryIds.Where(id => !foundIds.Contains(id)).ToList();
                    throw new NotFoundException($"Categories with IDs: {string.Join(", ", missingIds)} not found");
                }

                foreach (var category in categories)
                {
                    blogPost.Categories.Add(category);
                }
            }

            // Add tags if provided
            if (request.TagIds.Any())
            {
                var tagSpec = new SpecificationBuilder<Tag>()
                    .WhereIn(t => t.Id, request.TagIds)
                    .Build();

                var tags = await _tagRepository.FindTrackedAsync(tagSpec, cancellationToken);

                if (tags.Count != request.TagIds.Count)
                {
                    var foundIds = tags.Select(t => t.Id).ToHashSet();
                    var missingIds = request.TagIds.Where(id => !foundIds.Contains(id)).ToList();
                    throw new NotFoundException($"Tags with IDs: {string.Join(", ", missingIds)} not found");
                }

                foreach (var tag in tags)
                {
                    blogPost.Tags.Add(tag);
                }
            }

            // Save to database
            //await _blogPostRepository.CreateAsync(blogPost);

            await _sqlRepository.CreateAsync(blogPost);

            // Get the created post with includes
            var createdSpec = new SpecificationBuilder<Core.Entities.BlogPost>()
                .Where(b => b.Id == blogPost.Id)
                .Include(b => b.Categories)
                .Include(b => b.Tags)
                .Build();

            var createdPost = await _blogPostRepository.FirstOrDefaultAsync(createdSpec, cancellationToken);
            var responseDto = ManualMapper.MapToBlogPostResponseDto(createdPost!);

            return BaseResponse<BlogPostResponseDto>.SuccessResponse(responseDto, "Blog post created successfully");
        }

        //private string GenerateSlug(string title)
        //{
        //    var slug = title.ToLower().Trim();
        //    slug = slug.Replace(" ", "-");
        //    slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        //    slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-+", "-");
        //    return slug.Trim('-');
        //}
    }
}

