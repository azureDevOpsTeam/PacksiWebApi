using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.TelegramApis.Query;
using MediatR;

namespace ApplicationLayer.CQRS.TelegramApis.Handler;

public class UserAnyHandler(ITelegramServices telegramServices) : IRequestHandler<UserAnyQuery, HandlerResult>
{
    private readonly ITelegramServices _telegramServices = telegramServices;

    public async Task<HandlerResult> Handle(UserAnyQuery request, CancellationToken cancellationToken)
    {
        var user = await _telegramServices.UserAnyAsync(request.Model.TelegramUserId);
        return new HandlerResult { RequestStatus = user.RequestStatus, ObjectResult = user.Data, Message = user.Message };
    }
}