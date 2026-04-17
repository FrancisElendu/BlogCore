using BlogCore.Application.Features.Admin.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class AddMultipleRolesToUserCommandValidator : AbstractValidator<AddMultipleRolesToUserCommand>
    {
        public AddMultipleRolesToUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role is required")
                .Must(roles => roles.All(r => !string.IsNullOrWhiteSpace(r)))
                .WithMessage("Role names cannot be empty");
        }
    }
}
