using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Command;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class PublishedHandler(IRequestServices requestServices) : IRequestHandler<PublishedCommand, HandlerResult>

{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(PublishedCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.PublishedRequestAsync(requestDto.Model);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}