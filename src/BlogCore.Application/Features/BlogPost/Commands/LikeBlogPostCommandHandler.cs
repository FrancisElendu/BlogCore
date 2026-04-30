using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Enums;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class LikeBlogPostCommandHandler : IRequestHandler<LikeBlogPostCommand, BaseResponse<int>>
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public LikeBlogPostCommandHandler(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<BaseResponse<int>> Handle(LikeBlogPostCommand request, CancellationToken cancellationToken)
        {
            // Get existing blog post
            var blogPost = await _blogPostRepository.GetByIdAsync(request.Id, cancellationToken);

            if (blogPost == null)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Only published posts can be liked
            if (blogPost.Status != PostStatus.Published)
            {
                throw new BusinessRuleException($"Only published blog posts can be liked. Current status: {blogPost.Status}");
            }

            // Increment like count
            blogPost.LikeCount++;
            blogPost.UpdatedAt = DateTime.UtcNow;

            await _blogPostRepository.UpdateAsync(blogPost, cancellationToken);

            return BaseResponse<int>.SuccessResponse(blogPost.LikeCount, "Blog post liked successfully");
        }
    }
}
