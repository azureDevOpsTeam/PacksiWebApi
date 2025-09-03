using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_ConfirmedBySenderHandler(IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_ConfirmedBySenderCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;

    public async Task<HandlerResult> Handle(MiniApp_ConfirmedBySenderCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _miniAppServices.ConfirmedBySenderAsync(requestDto.Model);
        return result.ToHandlerResult();
    }
}