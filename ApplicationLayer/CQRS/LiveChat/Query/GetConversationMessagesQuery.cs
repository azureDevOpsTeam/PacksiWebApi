using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public record GetConversationMessagesQuery(int ConversationId, int Page = 1, int PageSize = 50) : IRequest<HandlerResult>;