using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Advertisements.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Handler;

public class PublishHandler(IUnitOfWork unitOfWork, IAdvertisementService advertisementService) : IRequestHandler<PublishCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public async Task<HandlerResult> Handle(PublishCommand request, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.PublishAsync(request.Model);
        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}