using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_SetPreferredCountriesHandler(IBotMessageServices botMessageServices) : IRequestHandler<MiniApp_SetPreferredCountriesCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(MiniApp_SetPreferredCountriesCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await botMessageServices.PreferredCountriesAsync(requestDto.TelegramId);
        return result.ToHandlerResult();
    }
}