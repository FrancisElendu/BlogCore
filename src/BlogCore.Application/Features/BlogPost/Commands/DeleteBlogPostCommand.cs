using BlogCore.Application.Common.Base;
using MediatR;

namespace BlogCore.Application.Features.BlogPost.Commands
{
    public class DeleteBlogPostCommand : IRequest<BaseResponse<bool>>
    {
        public Guid Id { get; set; }
    }
}
