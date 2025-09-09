using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record PickedUpCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;