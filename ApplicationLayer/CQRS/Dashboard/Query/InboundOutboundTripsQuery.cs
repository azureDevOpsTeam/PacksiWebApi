using MediatR;

namespace ApplicationLayer.CQRS.Dashboard.Query;

public record InboundOutboundTripsQuery : IRequest<HandlerResult>;