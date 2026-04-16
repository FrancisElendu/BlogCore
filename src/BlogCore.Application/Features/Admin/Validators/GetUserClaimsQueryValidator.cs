using BlogCore.Application.Features.Admin.Queries;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class GetUserClaimsQueryValidator : AbstractValidator<GetUserClaimsQuery>
    {
        public GetUserClaimsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
