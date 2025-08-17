using MediatR;

namespace ApplicationLayer.CQRS.Requests.Query;

public record OutboundTripsQuery : IRequest<HandlerResult>;