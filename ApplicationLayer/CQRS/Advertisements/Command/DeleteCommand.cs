using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Command;

public record DeleteCommand(KeyDto Model) : IRequest<HandlerResult>;