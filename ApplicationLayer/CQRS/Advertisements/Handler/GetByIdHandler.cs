using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Advertisements.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Handler;

public class GetByIdHandler(IAdvertisementService advertisementService) : IRequestHandler<GetByIdQuery, HandlerResult>
{
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public async Task<HandlerResult> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.GetByIdAsync(request.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}