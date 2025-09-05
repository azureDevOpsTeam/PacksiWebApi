using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_CreateRequestHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_CreateRequestCommand, HandlerResult>
{
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(MiniApp_CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);

        var resultAddRequest = await _miniAppServices.AddRequestAsync(request, userAccount.Value, cancellationToken);

        if (resultAddRequest.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = resultAddRequest.RequestStatus, Message = resultAddRequest.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
    }
}