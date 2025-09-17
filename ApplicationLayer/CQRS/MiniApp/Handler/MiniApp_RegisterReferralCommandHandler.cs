using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_RegisterReferralCommandHandler : IRequestHandler<MiniApp_RegisterReferralCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserAccountServices _userAccountServices;

    public MiniApp_RegisterReferralCommandHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices)
    {
        _unitOfWork = unitOfWork;
        _userAccountServices = userAccountServices;
    }

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

        return result.ToHandlerResult();
    }
}