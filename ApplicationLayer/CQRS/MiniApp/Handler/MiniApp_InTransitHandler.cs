using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_InTransitHandler(IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_InTransitCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;

    public async Task<HandlerResult> Handle(MiniApp_InTransitCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _miniAppServices.InTransitAsync(requestDto.Model);
        return result.ToHandlerResult();
    }
}