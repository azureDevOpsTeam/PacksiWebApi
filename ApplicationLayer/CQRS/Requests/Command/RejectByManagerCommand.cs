using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record RejectByManagerCommand(RequestKeyDto Model) : IRequest<HandlerResult>;
