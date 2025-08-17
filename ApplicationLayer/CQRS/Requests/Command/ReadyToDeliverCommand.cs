using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record ReadyToDeliverCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;