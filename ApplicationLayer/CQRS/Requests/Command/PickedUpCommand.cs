using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record PickedUpCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;