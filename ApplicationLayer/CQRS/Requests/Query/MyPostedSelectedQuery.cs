using MediatR;

namespace ApplicationLayer.CQRS.Requests.Query;
public record MyPostedSelectedQuery : IRequest<HandlerResult>;