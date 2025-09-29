using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_DepartureCountriesHandler(IUserAccountServices userAccountServices, IBotMessageServices botMessageServices) : IRequestHandler<MiniApp_DepartureCountriesCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_DepartureCountriesCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await userAccountServices.GetUserAccountByTelegramIdAsync(requestDto.TelegramId);
        if (resultValidation.IsFailure || resultValidation.Value == null)
            return resultValidation.ToHandlerResult();

        var result = await botMessageServices.DepartureCountriesAsync(requestDto.TelegramId);
        return result.ToHandlerResult();
    }
}