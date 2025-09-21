using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_GetDashboardDataHandler(
    IMiniAppServices miniAppServices,
    IUserAccountServices userAccountServices,
    IWalletService walletService
) : IRequestHandler<MiniApp_GetDashboardDataQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var resultValidation = await miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var resultCount = await userAccountServices.GetReferralCountAsync(resultValidation.Value.User.Id);
        if (resultCount.IsFailure)
            return resultCount.ToHandlerResult();

        var userAccount = await userAccountServices.GetUserAccountByTelegramIdAsync(resultValidation.Value.User.Id);
        if (userAccount.IsFailure)
            return userAccount.ToHandlerResult();

        var wallet = await walletService.GetBalanceAsync(userAccount.Value.Id);
        if (wallet.IsFailure)
            return wallet.ToHandlerResult();

        DashboardReportDto dashboardReportDto = new()
        {
            ReferralCount = resultCount.Value,
            IRRBalance = wallet.Value.IRR,
            USDTBalance = wallet.Value.USDT,
            TotalPackage = 0
        };

        return new HandlerResult
        {
            RequestStatus = RequestStatus.Successful,
            ObjectResult = dashboardReportDto,
            Message = CommonMessages.Successful
        };
    }
}