using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class RejectByManagerHandler(IRequestServices requestServices) : IRequestHandler<RejectByManagerCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(RejectByManagerCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.RejectByManagerAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}