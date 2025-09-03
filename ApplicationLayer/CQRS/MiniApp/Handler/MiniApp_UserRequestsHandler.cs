using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_UserRequestsHandler(IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_UserRequestsQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(MiniApp_UserRequestsQuery requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var result = await _miniAppServices.UserRequestsAsync(userAccount.Value);
        return result.ToHandlerResult();
    }
}