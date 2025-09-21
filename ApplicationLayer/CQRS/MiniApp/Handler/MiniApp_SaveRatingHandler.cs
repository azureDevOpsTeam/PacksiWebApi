using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_SaveRatingHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_SaveRatingCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(MiniApp_SaveRatingCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var result = await _miniAppServices.SaveRatingAsync(requestDto.Model, userAccount.Value);
        if (result.IsSuccess)
            await _unitOfWork.SaveChangesAsync();

        return result.ToHandlerResult();
    }
}