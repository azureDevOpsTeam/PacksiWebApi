using ApplicationLayer.DTOs.Requests;
using FluentValidation;

namespace ApplicationLayer.Validators.Requests
{
    public class DeliverableOriginLocationDtoValidator : AbstractValidator<DeliverableOriginLocationDto>
    {
        public DeliverableOriginLocationDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.OriginCityId.HasValue || !string.IsNullOrWhiteSpace(x.OriginDescription))
                .WithMessage("حداقل یکی از CityId یا توضیح مکان باید وارد شود.");

            RuleFor(x => x.OriginDescription).MaximumLength(500);
        }
    }
}