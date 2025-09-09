using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_PickedUpCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;