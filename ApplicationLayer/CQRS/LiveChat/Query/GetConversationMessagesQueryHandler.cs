using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Query;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<GetConversationMessagesQueryHandler> _logger;
    private readonly IUserContextService _userContextService;

    public GetConversationMessagesQueryHandler(ILiveChatServices liveChatServices, ILogger<GetConversationMessagesQueryHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.GetConversationMessagesAsync(request.ConversationId, new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value }, request.Page, request.PageSize);
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message,
                ObjectResult = result.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting conversation messages");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در دریافت پیام‌های مکالمه"
            };
        }
    }
}