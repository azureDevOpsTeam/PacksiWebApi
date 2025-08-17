using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Command;

public record AcceptCommand(KeyDto Model) : IRequest<HandlerResult>;