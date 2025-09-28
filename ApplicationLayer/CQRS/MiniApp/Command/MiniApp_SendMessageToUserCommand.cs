using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_SendMessageToUserCommand(long TelegramId, string Message) : IRequest<HandlerResult>;