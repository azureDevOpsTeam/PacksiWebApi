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

        await _unitOfWork.BeginTransactionAsync();
        var suggestion = await _miniAppServices.CreateSuggestionAsync(requestDto, userAccount.Value);
        if (suggestion.IsFailure)
        {
            await _unitOfWork.RollbackAsync();
            return suggestion.ToHandlerResult();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var suggestionAttachment = await _miniAppServices.CreateSuggestionAttachmentAsync(requestDto.Files, suggestion.Value.Id);
        if (suggestionAttachment.IsFailure)
        {
            await _unitOfWork.RollbackAsync();
            return suggestionAttachment.ToHandlerResult();
        }

        var result = await _miniAppServices.AddHistoryStatusAsync(suggestion.Value, RequestProcessStatus.Selected, userAccount.Value);
        if (result.IsFailure)
        {
            await _unitOfWork.RollbackAsync();
            return result.ToHandlerResult();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _unitOfWork.CommitAsync();

        return suggestion.ToHandlerResult();
    }
}