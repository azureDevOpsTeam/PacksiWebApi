using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_DepartureCountriesCommand(long TelegramId) : IRequest<HandlerResult>;