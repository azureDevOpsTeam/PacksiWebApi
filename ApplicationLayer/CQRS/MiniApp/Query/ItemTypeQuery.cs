using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record ItemTypeQuery : IRequest<HandlerResult>;