using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_RegisterReferralCommandHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices) : IRequestHandler<MiniApp_RegisterReferralCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(MiniApp_RegisterReferralCommand request, CancellationToken cancellationToken)
    {
        var existInvite = await _userAccountServices.GetExistReferralAsync(request.Model.TelegramUserId);

        if (existInvite != null)
            return existInvite.ToHandlerResult();

        var getInviter = await _userAccountServices.GetUserAccountInviterAsync(request.Model.ReferralCode);
        if (getInviter.IsSuccess)
        {
            var result = await _userAccountServices.AddReferralUserAsync(getInviter.Value.TelegramId.Value, request.Model);
            if (result.IsSuccess)
                await _unitOfWork.SaveChangesAsync();

            return result.ToHandlerResult();
        }

        return existInvite.ToHandlerResult();
    }
}