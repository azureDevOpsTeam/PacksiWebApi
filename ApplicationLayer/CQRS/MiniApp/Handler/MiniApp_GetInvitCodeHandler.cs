using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_GetInviteCodeHandler(
    IMiniAppServices miniAppServices,
    IUserAccountServices userAccountServices
) : IRequestHandler<MiniApp_GetInvitCodeQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_GetInvitCodeQuery request, CancellationToken cancellationToken)
    {
        var resultValidation = await miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        return new HandlerResult
        {
            RequestStatus = RequestStatus.Successful,
            ObjectResult = userAccount.Value.InviteCode,
            Message = CommonMessages.Successful
        };
    }
}