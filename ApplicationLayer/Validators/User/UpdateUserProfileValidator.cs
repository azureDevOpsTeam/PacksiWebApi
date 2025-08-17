using ApplicationLayer.DTOs.User;
using FluentValidation;

namespace ApplicationLayer.Validators.User
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(x => x.CountryOfResidenceId).GreaterThan(0);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        }
    }
}