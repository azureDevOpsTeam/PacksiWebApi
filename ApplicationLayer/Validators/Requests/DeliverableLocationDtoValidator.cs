using ApplicationLayer.DTOs.Requests;
using FluentValidation;

namespace ApplicationLayer.Validators.Requests
{
    public class DeliverableDestinationLocationDtoValidator : AbstractValidator<DeliverableDestinationLocationDto>
    {
        public DeliverableDestinationLocationDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.DestinationCityId.HasValue || !string.IsNullOrWhiteSpace(x.DestinationDescription))
                .WithMessage("حداقل یکی از CityId یا توضیح مکان باید وارد شود.");

            RuleFor(x => x.DestinationDescription).MaximumLength(500);
        }
    }
}