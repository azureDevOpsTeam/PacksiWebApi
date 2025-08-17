using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record DeliveredCommand(RequestKeyDto Model) : IRequest<HandlerResult>;