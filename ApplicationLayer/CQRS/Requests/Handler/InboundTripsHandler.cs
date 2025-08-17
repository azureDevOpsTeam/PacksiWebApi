using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class InboundTripsQueryHandler(IRequestServices requestServices) : IRequestHandler<InboundTripsQuery, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(InboundTripsQuery requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.InboundTripsQueryAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}