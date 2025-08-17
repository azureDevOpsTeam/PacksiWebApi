using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class ReadyToDeliverHandler(IRequestServices requestServices) : IRequestHandler<ReadyToDeliverCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(ReadyToDeliverCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.ReadyToDeliverAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}