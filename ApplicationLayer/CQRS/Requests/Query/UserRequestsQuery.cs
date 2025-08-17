using MediatR;

namespace ApplicationLayer.CQRS.Requests.Query;
public record UserRequestsQuery : IRequest<HandlerResult>;