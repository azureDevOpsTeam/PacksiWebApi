using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class AddUserDepartureLocationWithStarthandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBotMessageServices botMessageServices) : IRequestHandler<Command.AddUserDepartureLocationWithStartCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(Command.AddUserDepartureLocationWithStartCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await currentUserService.MiniApp_AddDepartureLocationAsync(requestDto.Model);
        if (result.IsSuccess)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        await botMessageServices.StepTwoAsync(requestDto.Model.TelegramId);
        return result.ToHandlerResult();
    }
}