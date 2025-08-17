using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record ReadyToPickupCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;