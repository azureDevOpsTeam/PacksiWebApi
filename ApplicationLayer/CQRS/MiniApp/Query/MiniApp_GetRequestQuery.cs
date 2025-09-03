using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_GetRequestQuery(RequestKeyDto Model) : IRequest<HandlerResult>;