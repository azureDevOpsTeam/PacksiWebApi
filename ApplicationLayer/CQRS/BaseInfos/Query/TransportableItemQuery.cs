using MediatR;

namespace ApplicationLayer.CQRS.BaseInfos.Query;

public record TransportableItemQuery : IRequest<HandlerResult>;