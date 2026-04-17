using BlogCore.Application.Features.Auth.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Auth.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters")
                .MaximumLength(100).WithMessage("Username must not exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("Username can only contain letters, numbers, underscores and hyphens");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(200).WithMessage("Email must not exceed 200 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters");

            RuleFor(x => x.DisplayName)
                .MaximumLength(100).WithMessage("Display name must not exceed 100 characters");
        }
    }
}
