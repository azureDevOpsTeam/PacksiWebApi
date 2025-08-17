using ApplicationLayer.DTOs.TelegramApis;
using MediatR;

namespace ApplicationLayer.CQRS.TelegramApis.Command;

public record VerifyTelegramCommand(TelegramUserInformationDto Model) : IRequest<HandlerResult>;