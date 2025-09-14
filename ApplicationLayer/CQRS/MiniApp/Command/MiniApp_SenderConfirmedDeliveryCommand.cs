using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_SenderConfirmedDeliveryCommand(RequestKeyDto Model) : IRequest<HandlerResult>;