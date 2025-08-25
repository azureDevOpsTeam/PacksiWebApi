using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class CreateRequestTMAHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUserAccountServices userAccountServices, IRequestServices requestServices, IMiniAppServices miniAppServices, IConfiguration configuration, ILogger<CreateRequestTMAHandler> logger) : IRequestHandler<CreateRequestTMACommand, HandlerResult>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IRequestServices _requestServices = requestServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<CreateRequestTMAHandler> _logger = logger;

    public async Task<HandlerResult> Handle(CreateRequestTMACommand request, CancellationToken cancellationToken)
    {
        //var botToken = _configuration["TelegramBot:Token"];
        var botToken = "8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A";

        var initData = _httpContextAccessor.HttpContext?.Request.Headers["X-Telegram-Init-Data"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(initData))
        {
            _logger.LogWarning("InitData is missing or empty");
            return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = "اطلاعات نامعتبر است" };
        }
        if (string.IsNullOrWhiteSpace(botToken))
        {
            _logger.LogError("Telegram bot token is not configured");
            return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = "اطلاعات نامعتبر است" };
        }
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);

        var resultAddRequest = await _requestServices.MiniApp_AddRequestAsync(request.Model, userAccount, cancellationToken);

        if (resultAddRequest.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = resultAddRequest.RequestStatus, Message = resultAddRequest.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var requestObj = (Request)resultAddRequest.Data;
        var resultAddStatus = await _requestServices.MiniApp_AddRequestSelectionAsync(requestObj.Id, userAccount, cancellationToken);

        if (resultAddStatus.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = resultAddStatus.RequestStatus, Message = resultAddStatus.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
    }
}