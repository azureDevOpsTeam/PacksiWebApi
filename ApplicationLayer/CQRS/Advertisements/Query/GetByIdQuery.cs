using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Query;

public record GetByIdQuery(KeyDto Model) : IRequest<HandlerResult>;