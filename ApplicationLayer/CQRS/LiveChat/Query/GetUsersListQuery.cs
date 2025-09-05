using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public record GetUsersListQuery() : IRequest<HandlerResult>;