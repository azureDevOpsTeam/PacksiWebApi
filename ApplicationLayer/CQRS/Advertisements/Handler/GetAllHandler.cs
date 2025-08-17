using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Advertisements.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Handler;

public class GetAllHandler(IAdvertisementService advertisementService) : IRequestHandler<GetAllQuery, HandlerResult>
{
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public async Task<HandlerResult> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.GetAllAsync(request.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}