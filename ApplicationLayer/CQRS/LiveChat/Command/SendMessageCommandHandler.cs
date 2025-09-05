using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Command;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<SendMessageCommandHandler> _logger;
    private readonly IUserContextService _userContextService;

    public SendMessageCommandHandler(ILiveChatServices liveChatServices, ILogger<SendMessageCommandHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.SendMessageAsync(request.Model, new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message,
                ObjectResult = result.Value
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending message");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در ارسال پیام"
            };
        }
    }
}