using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.CQRS.LiveChat.Command;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.LiveChat.Command;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, HandlerResult>
{
    private readonly ILiveChatServices _liveChatServices;
    private readonly ILogger<BlockUserCommandHandler> _logger;
    private readonly IUserContextService _userContextService;

    public BlockUserCommandHandler(ILiveChatServices liveChatServices, ILogger<BlockUserCommandHandler> logger, IUserContextService userContextService)
    {
        _liveChatServices = liveChatServices;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task<HandlerResult> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _liveChatServices.BlockUserAsync(request.Model, new DomainLayer.Entities.UserAccount { Id = _userContextService.UserId.Value });
            return new HandlerResult
            {
                RequestStatus = result.IsSuccess ? RequestStatus.Successful : RequestStatus.Failed,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while blocking user");
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = "خطا در مسدود کردن کاربر"
            };
        }
    }
}