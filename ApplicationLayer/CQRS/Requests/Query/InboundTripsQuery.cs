using MediatR;

namespace ApplicationLayer.CQRS.Requests.Query;

public record InboundTripsQuery : IRequest<HandlerResult>;