using BlogCore.Application.Features.BlogPost.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.BlogPost.Validations
{
    public class PublishBlogPostCommandValidator : AbstractValidator<PublishBlogPostCommand>
    {
        public PublishBlogPostCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Blog post ID is required");
        }
    }
}
