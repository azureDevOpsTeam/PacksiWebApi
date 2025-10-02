using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;
public record MiniApp_UserGuideQuery(long TelegramId, int userGuideType) : IRequest<HandlerResult>;