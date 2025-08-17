using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Command;

public record PublishCommand(KeyDto Model) : IRequest<HandlerResult>;