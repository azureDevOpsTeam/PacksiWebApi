namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IDashboardService
{
    Task<ServiceResult> InboundOutboundTripsAsync();

    Task<ServiceResult> ReportTripsAsync();
}