using ApplicationLayer.DTOs.Advertisements;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Query;

public record GetAllQuery(GetAllFilterDto Model) : IRequest<HandlerResult>;