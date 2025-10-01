using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_SetPreferredCountriesCommand(long TelegramId) : IRequest<HandlerResult>;