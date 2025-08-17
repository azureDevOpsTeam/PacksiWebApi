using ApplicationLayer.DTOs.Area;
using ApplicationLayer.Extensions.ServiceMessages;
using FluentValidation;

namespace ApplicationLayer.Common.Validations
{
    public class CountryValidator : AbstractValidator<CountryDto>
    {
        public CountryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .WithMessage(CommonValidateMessages.Required("نام کشور"))
                .MaximumLength(100).WithErrorCode(ValidationErrorCodes.MaxLength)
                .WithMessage(CommonValidateMessages.MaxLength("نام کشور", 100));

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithErrorCode(ValidationErrorCodes.NotNull)
                .WithMessage(CommonValidateMessages.Required("کد کشور"))
                .MaximumLength(5).WithErrorCode(ValidationErrorCodes.MaxLength)
                .WithMessage(CommonValidateMessages.MaxLength("کد کشور", 5));
        }
    }
}