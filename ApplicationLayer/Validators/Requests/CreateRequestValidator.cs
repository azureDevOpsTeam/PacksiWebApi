using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.Extensions.SmartEnums;
using FluentValidation;

namespace ApplicationLayer.Validators.Requests
{
    public class CreateRequestValidator : AbstractValidator<CreateRequestDto>
    {
        public CreateRequestValidator()
        {
            RuleFor(x => x.OriginCityId)
                .NotNull().WithMessage("شهر مبدا الزامی است.")
                .GreaterThan(0).WithMessage("شناسه شهر مبدا معتبر نیست.");

            RuleFor(x => x.DestinationCityId)
                .NotNull().WithMessage("شهر مقصد الزامی است.")
                .GreaterThan(0).WithMessage("شناسه شهر مقصد معتبر نیست.")
                .NotEqual(x => x.OriginCityId).WithMessage("مبدا و مقصد نباید یکسان باشند.");

            RuleFor(x => x.DepartureDate)
                .NotEmpty().WithMessage("تاریخ حرکت الزامی است.")
                .GreaterThan(DateTime.UtcNow).WithMessage("تاریخ حرکت باید در آینده باشد.");

            RuleFor(x => x.ArrivalDate)
                .NotEmpty().WithMessage("تاریخ رسیدن الزامی است.")
                .GreaterThan(x => x.DepartureDate).WithMessage("تاریخ رسیدن باید بعد از تاریخ حرکت باشد.");

            RuleFor(x => x.ItemTypeIds)
                .NotNull().WithMessage("نوع بار باید مشخص شود.")
                .Must(x => x.All(i => TransportableItemTypeEnum.TryFromValue(i, out _)))
                .WithMessage("یک یا چند نوع بار نامعتبر هستند.");

            RuleFor(x => x.RequestType)
                .Must(i => TransportableItemTypeEnum.TryFromValue(i, out _))
                .WithMessage("نوع درخواست نامعتبر است.");

            //RuleFor(x => x.MainOriginCityId).GreaterThan(0);
            //RuleFor(x => x.MainDestinationCityId).GreaterThan(0);

            //RuleForEach(x => x.AvailableOrigins).SetValidator(new DeliverableOriginLocationDtoValidator());
            //RuleForEach(x => x.AvailableDestinations).SetValidator(new DeliverableDestinationLocationDtoValidator());
        }
    }
}