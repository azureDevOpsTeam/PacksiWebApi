using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Query;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public class GetRecentConversationsQueryHandler : IRequestHandler<GetRecentConversationsQuery, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<GetRecentConversationsQueryHandler> _logger;
    private readonly IUserContextService _userContextService;

    public GetRecentConversationsQueryHandler(ILiveChatServices liveChatServices, ILogger<GetRecentConversationsQueryHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(GetRecentConversationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.GetRecentConversationsAsync(new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message,
                ObjectResult = result.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting recent conversations");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در دریافت مکالمات اخیر"
            };
        }
    }
}