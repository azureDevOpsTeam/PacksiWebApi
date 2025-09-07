using ApplicationLayer.DTOs.MiniApp;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_CreateSuggestionCommand(CreateSuggestionDto Model) : IRequest<HandlerResult>;