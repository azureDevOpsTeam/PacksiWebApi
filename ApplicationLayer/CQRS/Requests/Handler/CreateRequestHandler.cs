using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class CreateRequestHandler(IRequestServices requestServices, IUnitOfWork unitOfWork) : IRequestHandler<CreateRequestCommand, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var resultAddRequest = await _requestServices.AddRequestAsync(request, cancellationToken);

            if (resultAddRequest.RequestStatus != RequestStatus.Successful)
            {
                await _unitOfWork.RollbackAsync();
                return new HandlerResult { RequestStatus = resultAddRequest.RequestStatus, Message = resultAddRequest.Message };
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var requestObj = (Request)resultAddRequest.Data;

            var resultAddItemType = await _requestServices.AddRequestItemTypeAsync(request, requestObj.Id);
            if (resultAddItemType.RequestStatus != RequestStatus.Successful)
            {
                await _unitOfWork.RollbackAsync();
                return new HandlerResult { RequestStatus = resultAddItemType.RequestStatus, Message = resultAddItemType.Message };
            }

            var resultAddStatus = await _requestServices.AddRequestSelectionAsync(requestObj.Id, cancellationToken);
            if (resultAddStatus.RequestStatus != RequestStatus.Successful)
            {
                await _unitOfWork.RollbackAsync();
                return new HandlerResult { RequestStatus = resultAddStatus.RequestStatus, Message = resultAddStatus.Message };
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
            return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = CommonMessages.Failed };
        }
    }
}