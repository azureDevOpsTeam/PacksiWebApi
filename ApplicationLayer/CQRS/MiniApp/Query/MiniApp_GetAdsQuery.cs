using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_GetAdsQuery : IRequest<HandlerResult>;