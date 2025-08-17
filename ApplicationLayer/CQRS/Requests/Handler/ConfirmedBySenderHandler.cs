using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class ConfirmedBySenderHandler(IRequestServices requestServices) : IRequestHandler<ConfirmedBySenderCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(ConfirmedBySenderCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.ConfirmedBySenderAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}