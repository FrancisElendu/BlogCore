using BlogCore.Application.Common.Base;
using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Core.Enums;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class CreateBlogPostCommand : IRequest<BaseResponse<BlogPostResponseDto>>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string FeaturedImageUrl { get; set; } = string.Empty;
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public DateTime? ScheduledPublishDate { get; set; }
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public Guid AuthorId { get; set; }
    }
}
