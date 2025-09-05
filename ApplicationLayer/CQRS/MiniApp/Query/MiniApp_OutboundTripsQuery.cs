using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_GetRequestTripsQuery : IRequest<HandlerResult>;