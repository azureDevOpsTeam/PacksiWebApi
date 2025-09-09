using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record InTransitCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;