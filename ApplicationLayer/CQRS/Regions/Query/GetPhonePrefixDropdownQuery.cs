using MediatR;

namespace ApplicationLayer.CQRS.Regions.Query;

public record GetPhonePrefixDropdownQuery : IRequest<HandlerResult>;