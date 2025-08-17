using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Command;
public record PublishedCommand(RequestKeyDto Model) : IRequest<HandlerResult>;