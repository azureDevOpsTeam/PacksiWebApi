using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Advertisements.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Handler;

public class AcceptHandler(IUnitOfWork unitOfWork, IAdvertisementService advertisementService) : IRequestHandler<AcceptCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAdvertisementService _advertisementService = advertisementService;

    public async Task<HandlerResult> Handle(AcceptCommand request, CancellationToken cancellationToken)
    {
        var result = await _advertisementService.AcceptAsync(request.Model);
        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}