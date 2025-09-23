using MediatR;

namespace ApplicationLayer.CQRS.MiniAppDashboard.Query;

public record GetMyInvitedUsersQuery : IRequest<HandlerResult>;