using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_RejectSelectionCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;