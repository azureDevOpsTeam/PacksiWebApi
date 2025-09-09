using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_InTransitCommand(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;