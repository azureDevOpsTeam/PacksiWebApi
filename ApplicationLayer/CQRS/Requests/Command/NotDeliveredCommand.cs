using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record NotDeliveredCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;