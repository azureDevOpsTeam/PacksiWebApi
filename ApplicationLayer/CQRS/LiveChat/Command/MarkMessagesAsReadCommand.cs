using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public record MarkMessagesAsReadCommand(int ConversationId) : IRequest<HandlerResult>;