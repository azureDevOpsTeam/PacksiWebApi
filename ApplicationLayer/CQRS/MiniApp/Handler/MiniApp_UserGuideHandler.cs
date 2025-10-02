using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_UserGuideHandler(IBotMessageServices botMessageServices) : IRequestHandler<MiniApp_UserGuideQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_UserGuideQuery request, CancellationToken cancellationToken)
    {
        var result = await botMessageServices.UserGuideMessageAsync(request.TelegramId, request.userGuideType);
        return result.ToHandlerResult();
    }
}