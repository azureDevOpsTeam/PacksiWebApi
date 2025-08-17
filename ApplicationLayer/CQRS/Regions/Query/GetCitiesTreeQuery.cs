using ApplicationLayer.DTOs.Regions;
using MediatR;

namespace ApplicationLayer.CQRS.Regions.Query;

public record GetCitiesTreeQuery : IRequest<HandlerResult>;