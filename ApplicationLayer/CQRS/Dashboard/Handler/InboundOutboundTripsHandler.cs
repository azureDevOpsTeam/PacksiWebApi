using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Dashboard.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Dashboard.Handler;

public class InboundOutboundTripsHandler(IDashboardService dashboardService) : IRequestHandler<InboundOutboundTripsQuery, HandlerResult>
{
    private readonly IDashboardService _dashboardService = dashboardService;

    public async Task<HandlerResult> Handle(InboundOutboundTripsQuery reportTripsQuery, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.InboundOutboundTripsAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}