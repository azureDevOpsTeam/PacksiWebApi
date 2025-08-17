using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler
{
    public class OutboundTripsHandler(IRequestServices requestServices) : IRequestHandler<OutboundTripsQuery, HandlerResult>
    {
        private readonly IRequestServices _requestServices = requestServices;

        public async Task<HandlerResult> Handle(OutboundTripsQuery requestDto, CancellationToken cancellationToken)
        {
            var result = await _requestServices.OutboundTripsAsync();
            return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
        }
    }
}