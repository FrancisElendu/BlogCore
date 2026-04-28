using BlogCore.Application.Common.Base;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class PublishBlogPostCommand : IRequest<BaseResponse<bool>>
    {
        public Guid Id { get; set; }
    }
}
