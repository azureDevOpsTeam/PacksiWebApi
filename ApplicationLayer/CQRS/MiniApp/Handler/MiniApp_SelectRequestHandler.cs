using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_SelectRequestHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_SelectRequestCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(MiniApp_SelectRequestCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var resultSelected = await _miniAppServices.SelectedRequestAsync(requestDto.Model, userAccount.Value);
        if (resultSelected.IsSuccess)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _miniAppServices.AddHistoryStatusAsync(resultSelected.Value, RequestProcessStatus.Selected, userAccount.Value);
        if (result.IsSuccess)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result.ToHandlerResult();
    }
}