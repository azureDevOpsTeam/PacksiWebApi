using ApplicationLayer.DTOs.Advertisements;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Command;

public record CreateCommand(CreateAdvertisementDto Model) : IRequest<HandlerResult>;