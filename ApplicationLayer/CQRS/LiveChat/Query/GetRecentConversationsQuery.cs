using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public record GetRecentConversationsQuery() : IRequest<HandlerResult>;