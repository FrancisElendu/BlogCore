using BlogCore.Application.Common.Base;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class LikeBlogPostCommand : IRequest<BaseResponse<int>>
    {
        public Guid Id { get; set; }
    }
}
