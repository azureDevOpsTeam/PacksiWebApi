using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record GetReferralCountQuery : IRequest<HandlerResult>;