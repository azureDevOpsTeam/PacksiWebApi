using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;

public record CreateRequestCommand(CreateRequestDto Model) : IRequest<HandlerResult>;