using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.Managers.Command;

public record InviteUserCommand(InviteUserDto Model) : IRequest<HandlerResult>;