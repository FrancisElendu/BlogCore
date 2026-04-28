using BlogCore.Application.Features.BlogPost.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.BlogPost.Validations
{
    public class UpdateBlogPostCommandValidator : AbstractValidator<UpdateBlogPostCommand>
    {
        public UpdateBlogPostCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Blog post ID is required");

            RuleFor(x => x.Title)
                .Length(3, 200).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Title must be between 3 and 200 characters");

            RuleFor(x => x.Content)
                .MinimumLength(50).When(x => !string.IsNullOrEmpty(x.Content))
                .WithMessage("Content must be at least 50 characters");

            RuleFor(x => x.Excerpt)
                .MaximumLength(500).WithMessage("Excerpt cannot exceed 500 characters");

            RuleFor(x => x.FeaturedImageUrl)
                .MaximumLength(500).WithMessage("Featured image URL cannot exceed 500 characters")
                .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.FeaturedImageUrl))
                .WithMessage("Featured image URL must be a valid URL");
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
