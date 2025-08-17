using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Regions.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Handler
{
    public class GetPhonePrefixDropdownQueryHandler(IRegionServices regionServices) : IRequestHandler<GetPhonePrefixDropdownQuery, HandlerResult>
    {
        private readonly IRegionServices _regionServices = regionServices;

        public async Task<HandlerResult> Handle(GetPhonePrefixDropdownQuery requestDto, CancellationToken cancellationToken)
        {
            var result = await _regionServices.GetAllPhonePrefixAsync();
            return result.ToHandlerResult();
        }
    }
}