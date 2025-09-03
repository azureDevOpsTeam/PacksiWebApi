using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_RejectSelectionCommand(RequestSelectionKeyDto Model) : IRequest<HandlerResult>;