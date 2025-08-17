using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class RejectSelectionRequestHandler(IRequestServices requestServices) : IRequestHandler<RejectSelectionCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(RejectSelectionCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.RejectSelectionAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}