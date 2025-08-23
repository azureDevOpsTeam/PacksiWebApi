using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record UserInfoTMAQuery : IRequest<HandlerResult>;