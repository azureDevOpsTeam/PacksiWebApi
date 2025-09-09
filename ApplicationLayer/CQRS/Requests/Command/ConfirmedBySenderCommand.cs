using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record ConfirmedBySenderCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;