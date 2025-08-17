using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Query;
public record GetRequestQuery(RequestKeyDto Model) : IRequest<HandlerResult>;