using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Common.Mappings;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class UpdateBlogPostCommandHandler : IRequestHandler<UpdateBlogPostCommand, BaseResponse<BlogPostResponseDto>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;

        public UpdateBlogPostCommandHandler(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        public async Task<BaseResponse<BlogPostResponseDto>> Handle(UpdateBlogPostCommand request, CancellationToken cancellationToken)
        {
            // Get existing blog post with includes
            var existingSpec = new SpecificationBuilder<BlogCore.Core.Entities.BlogPost>()
                .Where(b => b.Id == request.Id)
                .Include(b => b.Categories)
                .Include(b => b.Tags)
                .Build();

            var blogPost = await _blogPostRepository.FirstOrDefaultAsync(existingSpec, cancellationToken);

            if (blogPost == null)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Check if blog post is in a modifiable state
            if (blogPost.Status == PostStatus.Deleted)
            {
                throw new BusinessRuleException($"Cannot update a deleted blog post (ID: {request.Id})");
            }

            if (blogPost.Status == PostStatus.Archived)
            {
                throw new BusinessRuleException($"Cannot update an archived blog post (ID: {request.Id}). Please unarchive first.");
            }

            // Generate new slug if title changed
            var slug = blogPost.Slug;
            if (blogPost.Title != request.Title)
            {
                slug = BaseGenerateSlug.GenerateSlug(request.Title);

                // Check for duplicate slug (excluding current post)
                var duplicateSlugSpec = new SpecificationBuilder<BlogCore.Core.Entities.BlogPost>()
                    .Where(b => b.Slug == slug && b.Id != request.Id)
                    .Build();

                var slugExists = await _blogPostRepository.AnyAsync(duplicateSlugSpec, cancellationToken);
                if (slugExists)
                {
                    throw new DuplicateException(nameof(BlogPost), "Slug", slug);
                }
            }

            // Update basic properties
            blogPost.Title = request.Title;
            blogPost.Slug = slug;
            blogPost.Content = request.Content;
            blogPost.Excerpt = request.Excerpt;
            blogPost.FeaturedImageUrl = request.FeaturedImageUrl;
            blogPost.UpdatedAt = DateTime.UtcNow;

            // Handle status change if provided
            if (request.Status.HasValue && request.Status.Value != blogPost.Status)
            {
                var oldStatus = blogPost.Status;
                blogPost.Status = request.Status.Value;

                // If publishing, set PublishedAt
                if (request.Status.Value == PostStatus.Published && oldStatus != PostStatus.Published)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }

                // If unpublishing from Published status
                if (request.Status.Value != PostStatus.Published && oldStatus == PostStatus.Published)
                {
                    // Keep PublishedAt for historical purposes
                    // Optionally could clear it, but better to keep for audit
                }
            }

            // Update categories
            blogPost.Categories.Clear();
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

            // Update tags
            blogPost.Tags.Clear();
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

            // Save changes
            await _blogPostRepository.UpdateAsync(blogPost);

            // Get updated post with includes for response
            var updatedSpec = new SpecificationBuilder<BlogCore.Core.Entities.BlogPost>()
                .Where(b => b.Id == request.Id)
                .Include(b => b.Categories)
                .Include(b => b.Tags)
                .Build();

            var updatedPost = await _blogPostRepository.FirstOrDefaultAsync(updatedSpec, cancellationToken);
            var responseDto = ManualMapper.MapToBlogPostResponseDto(updatedPost!);

            return BaseResponse<BlogPostResponseDto>.SuccessResponse(responseDto, "Blog post updated successfully");
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
