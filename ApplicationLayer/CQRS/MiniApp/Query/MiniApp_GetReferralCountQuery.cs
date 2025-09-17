using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_GetReferralCountQuery : IRequest<HandlerResult>;