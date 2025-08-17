using ApplicationLayer.DTOs.TelegramApis;
using MediatR;

namespace ApplicationLayer.CQRS.TelegramApis.Query;

public record UserAnyQuery(TelegramUserIdDto Model) : IRequest<HandlerResult>;