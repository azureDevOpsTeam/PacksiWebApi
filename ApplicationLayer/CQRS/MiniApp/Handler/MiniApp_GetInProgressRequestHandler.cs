using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_GetInProgressRequestHandler(IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_GetInProgressRequestQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(MiniApp_GetInProgressRequestQuery requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var result = await _miniAppServices.GetInProgressRequestAsync(userAccount.Value);
        return result.ToHandlerResult();
    }
}