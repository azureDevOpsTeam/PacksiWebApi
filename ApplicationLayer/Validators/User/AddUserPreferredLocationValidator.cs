using ApplicationLayer.DTOs.User;
using FluentValidation;

namespace ApplicationLayer.Validators.User
{
    public class AddUserPreferredLocationValidator : AbstractValidator<PreferredLocationDto>
    {
        public AddUserPreferredLocationValidator()
        {
            RuleFor(x => x.CountryId)
                .NotNull().When(x => x.CityId == null).WithMessage("شهر یا کشور را انتخاب کنید");
        }
    }
}