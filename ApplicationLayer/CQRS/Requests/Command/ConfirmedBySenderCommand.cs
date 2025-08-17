using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record ConfirmedBySenderCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;