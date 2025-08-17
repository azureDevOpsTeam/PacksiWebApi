using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record RejectSelectionCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;