using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class UserValicationHandler(IMiniAppServices miniAppServices, IConfiguration configuration, ILogger<UserValicationHandler> logger) : IRequestHandler<UserValicationQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UserValicationHandler> _logger = logger;

    public async Task<HandlerResult> Handle(UserValicationQuery request, CancellationToken cancellationToken)
    {
        var botToken = _configuration["TelegramBot:Token"];
        if (string.IsNullOrWhiteSpace(botToken))
        {
            _logger.LogError("Telegram bot token is not configured");
            return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = "تنظیمات ربات تلگرام یافت نشد" };
        }

        var user = await _miniAppServices.ValidateTelegramMiniAppUserAsync(request.Model.InitData, botToken);
        if (user.IsFailure)
        {
            return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = user.Error.Message };
        }

        return new HandlerResult { RequestStatus = RequestStatus.Successful, ObjectResult = user.Value };
    }
}