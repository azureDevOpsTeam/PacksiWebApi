using ApplicationLayer.DTOs.MiniApp;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record CreateRequestTMACommand(CreateRequestTMADto Model) : IRequest<HandlerResult>;