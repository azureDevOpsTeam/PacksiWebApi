using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_ConfirmedBySenderCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;