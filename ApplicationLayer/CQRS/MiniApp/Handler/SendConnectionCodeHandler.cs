using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class SendConnectionCodeHandler(IMiniAppServices miniAppServices) : IRequestHandler<SendConnectionCodeCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;

    public async Task<HandlerResult> Handle(SendConnectionCodeCommand request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var resultSendMessage = await _miniAppServices.SendMessageAsync(123);
        return resultValidation.ToHandlerResult();
    }
}