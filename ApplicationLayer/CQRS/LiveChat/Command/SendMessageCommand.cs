using ApplicationLayer.DTOs.LiveChat;
using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public record SendMessageCommand(SendMessageDto Model) : IRequest<HandlerResult>;