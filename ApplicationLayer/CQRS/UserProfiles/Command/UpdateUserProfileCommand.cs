using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Command;

public record UpdateUserProfileCommand(UpdateUserProfileDto Model) : IRequest<HandlerResult>;