using ApplicationLayer.DTOs.LiveChat;
using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public record BlockUserCommand(BlockUserDto Model) : IRequest<HandlerResult>;