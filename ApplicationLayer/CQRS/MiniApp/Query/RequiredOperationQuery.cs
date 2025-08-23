using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record RequiredOperationQuery : IRequest<HandlerResult>;