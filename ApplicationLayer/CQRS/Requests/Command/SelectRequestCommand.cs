using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;

public record SelectRequestCommand(RequestKeyDto Model) : IRequest<HandlerResult>;