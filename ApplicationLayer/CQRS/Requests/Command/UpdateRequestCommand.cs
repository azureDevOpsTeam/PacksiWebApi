using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;

public record UpdateRequestCommand(UpdateRequestDto Model) : IRequest<HandlerResult>;