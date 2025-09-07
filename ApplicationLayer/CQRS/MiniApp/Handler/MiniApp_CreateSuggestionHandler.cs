using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_CreateSuggestionHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_CreateSuggestionCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(MiniApp_CreateSuggestionCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var suggestion = await _miniAppServices.CreateSuggestionAsync(requestDto.Model, userAccount.Value);
        if (suggestion.IsFailure)
            return suggestion.ToHandlerResult();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _miniAppServices.AddHistoryStatusAsync(suggestion.Value, RequestProcessStatus.Selected, userAccount.Value);
        if (result.IsFailure)
            return result.ToHandlerResult();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return suggestion.ToHandlerResult();
    }
}