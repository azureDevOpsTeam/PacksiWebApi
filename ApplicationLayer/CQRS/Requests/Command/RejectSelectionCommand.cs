using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record RejectSelectionCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;