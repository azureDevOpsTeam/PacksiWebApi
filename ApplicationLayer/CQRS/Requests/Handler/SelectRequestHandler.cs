using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class SelectRequestHandler(IRequestServices requestServices) : IRequestHandler<SelectRequestCommand, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(SelectRequestCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.SelectedRequestAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}