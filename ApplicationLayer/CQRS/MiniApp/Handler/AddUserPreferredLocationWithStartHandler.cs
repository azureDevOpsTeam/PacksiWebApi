using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class AddUserPreferredLocationWithStartHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBotMessageServices botMessageServices) : IRequestHandler<AddUserPreferredLocationWithStartCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(AddUserPreferredLocationWithStartCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await currentUserService.MiniApp_AddDepartureLocationAsync(requestDto.Model);
        if (result.IsSuccess)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        await botMessageServices.StepTwoAsync(requestDto.Model.TelegramId);
        return result.ToHandlerResult();
    }
}