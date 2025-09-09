using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record ReadyToDeliverCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;