using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.BusinessLogic.Services;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public class GetUsersListQueryHandler(IUserAccountServices userAccountServices, IMiniAppServices miniAppServices, ILiveChatServices liveChatServices, ILogger<GetUsersListQueryHandler> logger) : IRequestHandler<GetUsersListQuery, HandlerResult>
{
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly ILiveChatServices _liveChatServices = liveChatServices;
    private readonly ILogger<GetUsersListQueryHandler> _logger = logger;

    public async Task<HandlerResult> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var result = await _liveChatServices.GetUsersListAsync(userAccount.Value);
        return result.ToHandlerResult();
    }
}