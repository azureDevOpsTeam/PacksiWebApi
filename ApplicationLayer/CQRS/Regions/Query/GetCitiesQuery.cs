using ApplicationLayer.DTOs.Regions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Query;

public record GetCitiesQuery(CountryKeyDto Model) : IRequest<HandlerResult>;