using MediatR;

namespace ApplicationLayer.CQRS.Dashboard.Query;
public record ReportTripsQuery : IRequest<HandlerResult>;