using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record AddUserPreferredLocationTMACommand(PreferredLocationDto Model) : IRequest<HandlerResult>;