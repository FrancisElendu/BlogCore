using BlogCore.Application.Features.Admin.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class SyncUserClaimsCommandValidator : AbstractValidator<SyncUserClaimsCommand>
    {
        public SyncUserClaimsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
