using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_GetRequestHandler(IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_GetRequestQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;

    public async Task<HandlerResult> Handle(MiniApp_GetRequestQuery requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _miniAppServices.GetRequestByIdAsync(requestDto.Model);
        return result.ToHandlerResult();
    }
}