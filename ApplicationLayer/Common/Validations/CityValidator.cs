using ApplicationLayer.DTOs.Area;
using ApplicationLayer.Extensions.ServiceMessages;
using FluentValidation;

namespace ApplicationLayer.Common.Validations
{
    public class CityValidator : AbstractValidator<CityDto>
    {
        public CityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .WithMessage(CommonValidateMessages.Required("نام شهر"))
                .MaximumLength(100).WithErrorCode(ValidationErrorCodes.MaxLength)
                .WithMessage(CommonValidateMessages.MaxLength("نام شهر", 100));

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithErrorCode(ValidationErrorCodes.MustBeGreaterThanZero)
                .WithMessage(CommonValidateMessages.MustBeGreaterThanZero("آیدی کشور"));
        }
    }
}