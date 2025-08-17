using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class UpdateRequestHandler(IRequestServices requestServices, IUnitOfWork unitOfWork) : IRequestHandler<UpdateRequestCommand, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(UpdateRequestCommand request, CancellationToken cancellationToken)
    {
        var result = await _requestServices.UpdateRequestAsync(request.Model, cancellationToken);

        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
    }
}