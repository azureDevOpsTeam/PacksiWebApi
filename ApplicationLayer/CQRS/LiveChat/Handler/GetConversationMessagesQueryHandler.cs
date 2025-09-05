using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Query;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Handler;

public class GetConversationMessagesQueryHandler(IUserAccountServices userAccountServices, IMiniAppServices miniAppServices, ILiveChatServices liveChatServices, ILogger<GetUsersListQueryHandler> logger) : IRequestHandler<GetConversationMessagesQuery, HandlerResult>
{
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly ILiveChatServices _liveChatServices = liveChatServices;
    private readonly ILogger<GetUsersListQueryHandler> _logger = logger;

    public async Task<HandlerResult> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var result = await _liveChatServices.GetConversationMessagesAsync(request.ConversationId, userAccount.Value, request.Page, request.PageSize);
        return new HandlerResult
        {
            RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
            Message = result.Message,
            ObjectResult = result.Value
        };
    }
}