using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_RegisterReferralCommandHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IWalletService walletService) : IRequestHandler<MiniApp_RegisterReferralCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IWalletService _walletService = walletService;

    public async Task<HandlerResult> Handle(MiniApp_RegisterReferralCommand request, CancellationToken cancellationToken)
    {
        var existInvite = await _userAccountServices.GetExistReferralAsync(request.Model.TelegramUserId);

        if (!existInvite.IsSuccess)
            return existInvite.ToHandlerResult();

        var getInviter = await _userAccountServices.GetUserAccountInviterAsync(request.Model.ReferralCode);
        if (!getInviter.IsSuccess)
            return getInviter.ToHandlerResult();

        var inviterTelegramId = getInviter.Value.TelegramId;
        if (!inviterTelegramId.HasValue)
            return Result.GeneralFailure("Inviter has no TelegramId").ToHandlerResult();

        if (inviterTelegramId.Value == request.Model.TelegramUserId)
            return Result.GeneralFailure("Self-referral is not allowed").ToHandlerResult();

        var result = await _userAccountServices.AddReferralUserAsync(inviterTelegramId.Value, request.Model);
        if (result.IsSuccess)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        decimal bonusAmount = 0.01m;
        var addTransaction = await _walletService.CreditAsync(getInviter.Value.Id, CurrencyEnum.USDT, bonusAmount, TransactionTypeEnum.Bonus);
        if (addTransaction.IsFailure)
            return addTransaction.ToHandlerResult();

        return result.ToHandlerResult();
    }
}