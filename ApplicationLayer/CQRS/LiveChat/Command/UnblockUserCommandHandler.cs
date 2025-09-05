using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Command;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<UnblockUserCommandHandler> _logger;
    private readonly IUserContextService _userContextService;

    public UnblockUserCommandHandler(ILiveChatServices liveChatServices, ILogger<UnblockUserCommandHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.UnblockUserAsync(request.UserId, new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while unblocking user");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در آنبلاک کردن کاربر"
            };
        }
    }
}