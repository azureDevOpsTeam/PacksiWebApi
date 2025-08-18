using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record SendConnectionCodeCommand(TelegramIdDto Model) : IRequest<HandlerResult>;