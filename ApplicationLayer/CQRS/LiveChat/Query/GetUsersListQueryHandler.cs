using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Query;

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<GetUsersListQueryHandler> _logger;
    private readonly IUserContextService _userContextService;

    public GetUsersListQueryHandler(ILiveChatServices liveChatServices, ILogger<GetUsersListQueryHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.GetUsersListAsync(new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return result.ToHandlerResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting users list");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در دریافت لیست کاربران"
            };
        }
    }
}