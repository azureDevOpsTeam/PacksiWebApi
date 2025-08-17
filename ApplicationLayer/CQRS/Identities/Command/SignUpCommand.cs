using ApplicationLayer.DTOs.Identity;
using MediatR;

namespace ApplicationLayer.CQRS.Identities.Command;

public record SignUpCommand(SignUpDto Model) : IRequest<HandlerResult>;