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

public class CreateRequestTMAHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IUserAccountServices userAccountServices, IRequestServices requestServices, IMiniAppServices miniAppServices, IConfiguration configuration, ILogger<CreateRequestTMAHandler> logger) : IRequestHandler<MiniApp_CreateRequestCommand, HandlerResult>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IRequestServices _requestServices = requestServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<CreateRequestTMAHandler> _logger = logger;

    public async Task<HandlerResult> Handle(MiniApp_CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);

        var resultAddRequest = await _requestServices.MiniApp_AddRequestAsync(request, userAccount.Value, cancellationToken);

        if (resultAddRequest.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = resultAddRequest.RequestStatus, Message = resultAddRequest.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var requestObj = (Request)resultAddRequest.Data;
        var resultAddStatus = await _requestServices.MiniApp_AddRequestSelectionAsync(requestObj.Id, userAccount.Value, cancellationToken);

        if (resultAddStatus.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = resultAddStatus.RequestStatus, Message = resultAddStatus.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
    }
}