using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Query;

public record UserInfoQuery : IRequest<HandlerResult>;