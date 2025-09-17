using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_GetReferralCountHandler(
    IMiniAppServices miniAppServices,
    IUserAccountServices userAccountServices
) : IRequestHandler<MiniApp_GetReferralCountQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_GetReferralCountQuery request, CancellationToken cancellationToken)
    {
        var resultValidation = await miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var resultCount = await userAccountServices.GetReferralCountAsync(resultValidation.Value.User.Id);
        if (resultCount.IsFailure)
            return resultCount.ToHandlerResult();

        DashboardReportDto dashboardReportDto = new()
        {
            ReferralCount = resultCount.Value,
            IRRBalance = 0,
            USDTBalance = 0,
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