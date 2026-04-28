using BlogCore.Application.Features.BlogPost.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.BlogPost.Validations
{
    public class DeleteBlogPostCommandValidator : AbstractValidator<DeleteBlogPostCommand>
    {
        public DeleteBlogPostCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Blog post ID is required");
        }
    }
}
