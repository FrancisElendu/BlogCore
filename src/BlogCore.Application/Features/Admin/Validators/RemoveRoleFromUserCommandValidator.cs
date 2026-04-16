using BlogCore.Application.Features.Admin.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
    {
        public RemoveRoleFromUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role name is required")
                .MinimumLength(2).WithMessage("Role name must be at least 2 characters")
                .MaximumLength(50).WithMessage("Role name must not exceed 50 characters");
        }
    }
}
