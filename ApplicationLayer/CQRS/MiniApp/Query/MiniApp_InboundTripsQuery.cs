using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_InboundTripsQuery : IRequest<HandlerResult>;