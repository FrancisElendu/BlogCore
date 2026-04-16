using BlogCore.Application.Features.Admin.Queries;
using FluentValidation;

namespace BlogCore.Application.Features.Admin.Validators
{
    public class GetUserRolesQueryValidator : AbstractValidator<GetUserRolesQuery>
    {
        public GetUserRolesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
