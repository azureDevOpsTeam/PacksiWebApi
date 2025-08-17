using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Advertisements.Command;

public record RejectCommand(KeyDto Model) : IRequest<HandlerResult>;