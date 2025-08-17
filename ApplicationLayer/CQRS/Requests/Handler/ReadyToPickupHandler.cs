using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class ReadyToPickupHandler(IRequestServices requestServices) : IRequestHandler<ReadyToPickupCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(ReadyToPickupCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.ReadyToPickupAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}