using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniAppDashboard.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniAppDashboard.Handler;

public class GetMyInvitedUsersHandler(IMiniAppServices miniAppServices, IUserAccountServices userAccountServices) : IRequestHandler<GetMyInvitedUsersQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(GetMyInvitedUsersQuery requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _userAccountServices.GetMyInvitedUsersAsync(resultValidation.Value.User.Id);
        return result.ToHandlerResult();
    }
}