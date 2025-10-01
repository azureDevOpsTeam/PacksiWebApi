using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_SetPreferredCountriesHandler(IUserAccountServices userAccountServices, IBotMessageServices botMessageServices) : IRequestHandler<MiniApp_SetPreferredCountriesCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_SetPreferredCountriesCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await userAccountServices.GetUserAccountByTelegramIdAsync(requestDto.TelegramId);
        if (resultValidation.IsFailure || resultValidation.Value == null)
            return resultValidation.ToHandlerResult();

        var result = await botMessageServices.PreferredCountriesAsync(requestDto.TelegramId);
        return result.ToHandlerResult();
    }
}