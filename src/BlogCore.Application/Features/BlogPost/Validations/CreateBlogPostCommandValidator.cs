using BlogCore.Application.Features.BlogPost.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.BlogPost.Validations
{
    public class CreateBlogPostCommandValidator : AbstractValidator<CreateBlogPostCommand>
    {
        public CreateBlogPostCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(50).WithMessage("Content must be at least 50 characters");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("Author ID is required");

            RuleFor(x => x.Excerpt)
                .MaximumLength(500).WithMessage("Excerpt cannot exceed 500 characters");

            RuleFor(x => x.FeaturedImageUrl)
                .MaximumLength(500).WithMessage("Featured image URL cannot exceed 500 characters")
                .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.FeaturedImageUrl))
                .WithMessage("Featured image URL must be a valid URL");

            RuleFor(x => x.ScheduledPublishDate)
                .Must(BeFutureDate).When(x => x.ScheduledPublishDate.HasValue)
                .WithMessage("Scheduled publish date must be in the future");
        }

        private bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        private bool BeFutureDate(DateTime? date)
        {
            return date > DateTime.UtcNow;
        }
    }
}
