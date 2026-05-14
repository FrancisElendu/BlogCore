using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Enums;
using MediatR;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class DeleteBlogPostCommandHandler : IRequestHandler<DeleteBlogPostCommand, BaseResponse<bool>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IRepository<BlogCore.Core.Entities.BlogPost> _sqlRepository;

        public DeleteBlogPostCommandHandler(IBlogPostRepository blogPostRepository, IRepository<BlogCore.Core.Entities.BlogPost> sqlRepository)
        {
            _blogPostRepository = blogPostRepository;
            _sqlRepository = sqlRepository;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteBlogPostCommand request, CancellationToken cancellationToken)
        {
            // Get existing blog post
            //var blogPost = await _blogPostRepository.GetByIdAsync(request.Id, cancellationToken);
            var blogPost = await _sqlRepository.GetByIdAsync(request.Id);

            if (blogPost == null)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Check if already deleted
            if (blogPost.Status == PostStatus.Deleted)
            {
                throw new BusinessRuleException($"Blog post (ID: {request.Id}) is already deleted.");
            }

            // Soft delete - mark as deleted instead of actually removing from database
            blogPost.Status = PostStatus.Deleted;
            blogPost.UpdatedAt = DateTime.UtcNow;

            //await _blogPostRepository.UpdateAsync(blogPost, cancellationToken);
            await _sqlRepository.UpdateAsync(blogPost);

            // Note: If you want hard delete instead, use:
            // await _blogPostRepository.DeleteAsync(request.Id, cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Blog post deleted successfully");
        }
    }
}
