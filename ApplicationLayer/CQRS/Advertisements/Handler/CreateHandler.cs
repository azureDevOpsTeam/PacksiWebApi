using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Advertisements.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Handler;

public class CreateHandler(IAdvertisementService advertisementService) : IRequestHandler<CreateCommand, HandlerResult>
{
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public async Task<HandlerResult> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.CreateAsync(request.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}