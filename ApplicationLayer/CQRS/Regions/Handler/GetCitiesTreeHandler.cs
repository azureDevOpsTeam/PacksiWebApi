using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Regions.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Handler;

public class GetCitiesTreeHandler(IRegionServices regionServices) : IRequestHandler<GetCitiesTreeQuery, HandlerResult>
{
    private readonly IRegionServices _regionServices = regionServices;

    public async Task<HandlerResult> Handle(GetCitiesTreeQuery request, CancellationToken cancellationToken)
    {
        var result = await _regionServices.GetCitiesTreeAsync();
        return result.ToHandlerResult();
    }
}