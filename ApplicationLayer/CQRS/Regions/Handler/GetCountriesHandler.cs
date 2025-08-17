using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Regions.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Handler;

public class GetCountriesHandler(IRegionServices regionServices) : IRequestHandler<GetCountriesQuery, HandlerResult>
{
    private readonly IRegionServices _regionServices = regionServices;

    public async Task<HandlerResult> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionServices.GetCountriesAsync();
        return result.ToHandlerResult();
    }
}