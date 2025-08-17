using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Regions.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Handler;

public class GetCitiesHandler(IRegionServices regionServices) : IRequestHandler<GetCitiesQuery, HandlerResult>
{
    private readonly IRegionServices _regionServices = regionServices;

    public async Task<HandlerResult> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionServices.GetCitiesAsync(request.Model);
        return result.ToHandlerResult();
    }
}