using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Command;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<MarkMessagesAsReadCommandHandler> _logger;
    private readonly IUserContextService _userContextService;

    public MarkMessagesAsReadCommandHandler(ILiveChatServices liveChatServices, ILogger<MarkMessagesAsReadCommandHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.MarkMessagesAsReadAsync(request.ConversationId, new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking messages as read");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در علامت‌گذاری پیام‌ها به عنوان خوانده شده"
            };
        }
    }
}