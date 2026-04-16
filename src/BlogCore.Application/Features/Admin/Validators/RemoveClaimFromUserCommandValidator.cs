using BlogCore.Application.Features.Admin.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class RemoveClaimFromUserCommandValidator : AbstractValidator<RemoveClaimFromUserCommand>
    {
        public RemoveClaimFromUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.ClaimType)
                .NotEmpty().WithMessage("Claim type is required")
                .MaximumLength(100).WithMessage("Claim type must not exceed 100 characters");

            RuleFor(x => x.ClaimValue)
                .NotEmpty().WithMessage("Claim value is required")
                .MaximumLength(200).WithMessage("Claim value must not exceed 200 characters");
        }
    }
}
