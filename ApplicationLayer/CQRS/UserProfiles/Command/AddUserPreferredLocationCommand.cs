using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Command;

public record AddUserPreferredLocationCommand(AddUserPreferredLocationDto Model) : IRequest<HandlerResult>;