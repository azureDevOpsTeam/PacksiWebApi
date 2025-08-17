using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Dashboard.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Dashboard.Handler;

public class ReportTripsHandler(IDashboardService dashboardService) : IRequestHandler<ReportTripsQuery, HandlerResult>
{
    private readonly IDashboardService _dashboardService = dashboardService;

    public async Task<HandlerResult> Handle(ReportTripsQuery reportTripsQuery, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.ReportTripsAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}