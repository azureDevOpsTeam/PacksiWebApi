using ApplicationLayer.DTOs.Commissions;
using FluentValidation;

namespace ApplicationLayer.Validators.Commissions
{
    public class CreateCommissionDtoValidator : AbstractValidator<CreateCommissionDto>
    {
        public CreateCommissionDtoValidator()
        {
            RuleFor(x => x.RequestId).GreaterThan(0);
            RuleFor(x => x.CarrierUserId).GreaterThan(0);
            RuleFor(x => x.RequestPrice).GreaterThan(0).WithMessage("قیمت باید بزرگتر از صفر باشد.");
        }
    }
}