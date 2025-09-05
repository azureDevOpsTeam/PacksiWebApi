using MediatR;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public class UnblockUserCommand : IRequest<HandlerResult>
{
    public int UserId { get; set; }
}