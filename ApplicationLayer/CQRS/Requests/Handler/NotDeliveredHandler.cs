using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class NotDeliveredHandler(IRequestServices requestServices) : IRequestHandler<NotDeliveredCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(NotDeliveredCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.NotDeliveredAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}