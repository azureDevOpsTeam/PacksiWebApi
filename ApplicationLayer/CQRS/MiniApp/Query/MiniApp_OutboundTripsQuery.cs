using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_OutboundTripsQuery : IRequest<HandlerResult>;