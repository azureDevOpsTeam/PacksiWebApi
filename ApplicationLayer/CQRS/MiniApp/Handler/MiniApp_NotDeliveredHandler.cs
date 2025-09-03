using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_NotDeliveredHandler(IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_NotDeliveredCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;

    public async Task<HandlerResult> Handle(MiniApp_NotDeliveredCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _miniAppServices.NotDeliveredAsync(requestDto.Model);
        return result.ToHandlerResult();
    }
}