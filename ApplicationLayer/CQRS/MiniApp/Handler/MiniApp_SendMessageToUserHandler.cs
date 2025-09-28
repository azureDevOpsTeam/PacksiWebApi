using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_SendMessageToUserHandler(IBotMessageServices botMessageServices)
    : IRequestHandler<MiniApp_SendMessageToUserCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_SendMessageToUserCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await botMessageServices.SendWelcomeMessageAsync(requestDto.TelegramId);
        return result.ToHandlerResult();
    }
}