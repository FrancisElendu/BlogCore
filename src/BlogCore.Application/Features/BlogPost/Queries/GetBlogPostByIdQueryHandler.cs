using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using BlogCore.Application.Common.Mappings;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.Interfaces;
using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;
using MediatR;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Features.BlogPost.Queries
{
    public class GetBlogPostByIdQueryHandler : IRequestHandler<GetBlogPostByIdQuery, BaseResponse<BlogPostResponseDto>>
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IRepository<BlogCore.Core.Entities.BlogPost> _sqlRepository;

        public GetBlogPostByIdQueryHandler(IBlogPostRepository blogPostRepository, IRepository<BlogCore.Core.Entities.BlogPost> sqlRepository)
        {
            _blogPostRepository = blogPostRepository;
            _sqlRepository = sqlRepository;
        }

        public async Task<BaseResponse<BlogPostResponseDto>> Handle(GetBlogPostByIdQuery request, CancellationToken cancellationToken)
        {
            // Build specification with includes
            var spec = new SpecificationBuilder<Core.Entities.BlogPost>()
                .Where(b => b.Id == request.Id)
                .Include(b => b.Categories)
                .Include(b => b.Tags)
                .Include(b => b.Comments)
                .Build();

            var blogPost = await _blogPostRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (blogPost == null)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Check if post is accessible
            if (blogPost.Status == PostStatus.Deleted)
            {
                throw new NotFoundException(nameof(BlogPost), request.Id);
            }

            // Increment view count (only for published posts)
            if (blogPost.Status == PostStatus.Published)
            {
                blogPost.ViewCount++;
                //await _blogPostRepository.UpdateAsync(blogPost, cancellationToken);
                await _sqlRepository.UpdateAsync(blogPost);
            }

            var responseDto = ManualMapper.MapToBlogPostResponseDto(blogPost);

            return BaseResponse<BlogPostResponseDto>.SuccessResponse(responseDto);
        }
    }
}
