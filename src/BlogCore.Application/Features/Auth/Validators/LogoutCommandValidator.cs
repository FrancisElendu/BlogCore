using BlogCore.Application.Features.Auth.Commands;
using FluentValidation;

namespace BlogCore.Application.Features.Auth.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required");
        }
    }
}
