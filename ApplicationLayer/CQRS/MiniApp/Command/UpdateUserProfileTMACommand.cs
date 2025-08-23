using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record UpdateUserProfileTMACommand(UpdateUserProfileDto Model) : IRequest<HandlerResult>;