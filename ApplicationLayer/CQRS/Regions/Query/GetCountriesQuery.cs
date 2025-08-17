using MediatR;

namespace ApplicationLayer.CQRS.Regions.Query;

public record GetCountriesQuery : IRequest<HandlerResult>;