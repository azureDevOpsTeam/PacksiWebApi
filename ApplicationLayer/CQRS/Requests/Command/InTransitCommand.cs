using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record InTransitCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;