using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Enums;
using MediatR;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class PublishBlogPostCommandHandler : IRequestHandler<PublishBlogPostCommand, BaseResponse<bool>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IRepository<BlogCore.Core.Entities.BlogPost> _sqlRepository;

        public PublishBlogPostCommandHandler(IBlogPostRepository blogPostRepository, IRepository<BlogCore.Core.Entities.BlogPost> sqlRepository)
        {
            _blogPostRepository = blogPostRepository;
            _sqlRepository = sqlRepository;
        }

        public async Task<BaseResponse<bool>> Handle(PublishBlogPostCommand request, CancellationToken cancellationToken)
        {
            // Get existing blog post
            //var blogPost = await _blogPostRepository.GetByIdAsync(request.Id, cancellationToken);
            var blogPost = await _sqlRepository.GetByIdAsync(request.Id);

            if (blogPost == null)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Validate current status
            if (blogPost.Status == PostStatus.Published)
            {
                throw new BusinessRuleException($"Blog post (ID: {request.Id}) is already published.");
            }

            if (blogPost.Status == PostStatus.Deleted)
            {
                throw new BusinessRuleException($"Cannot publish a deleted blog post (ID: {request.Id}).");
            }

            if (blogPost.Status == PostStatus.Archived)
            {
                throw new BusinessRuleException($"Cannot publish an archived blog post (ID: {request.Id}). Please unarchive first.");
            }

            // Validate content requirements for publishing
            if (string.IsNullOrWhiteSpace(blogPost.Title))
            {
                throw new BusinessRuleException($"Cannot publish blog post (ID: {request.Id}) without a title.");
            }

            if (string.IsNullOrWhiteSpace(blogPost.Content))
            {
                throw new BusinessRuleException($"Cannot publish blog post (ID: {request.Id}) with empty content.");
            }

            if (blogPost.Content.Length < 50)
            {
                throw new BusinessRuleException($"Cannot publish blog post (ID: {request.Id}) with content less than 50 characters.");
            }

            // Ensure at least one category (business rule)
            if (!blogPost.Categories.Any())
            {
                throw new BusinessRuleException($"Cannot publish blog post (ID: {request.Id}) without at least one category.");
            }

            // Publish the blog post
            blogPost.Status = PostStatus.Published;
            blogPost.PublishedAt = DateTime.UtcNow;
            blogPost.UpdatedAt = DateTime.UtcNow;

            //await _blogPostRepository.UpdateAsync(blogPost, cancellationToken);
            await _sqlRepository.UpdateAsync(blogPost);

            return BaseResponse<bool>.SuccessResponse(true, "Blog post published successfully");
        }
    }
}
