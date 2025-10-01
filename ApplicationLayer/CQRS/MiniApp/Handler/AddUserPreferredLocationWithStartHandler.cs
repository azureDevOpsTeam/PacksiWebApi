using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class AddUserPreferredLocationWithStartHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IBotMessageServices botMessageServices) : IRequestHandler<Command.AddUserPreferredLocationWithStartCommand, HandlerResult>
{
    public async Task<HandlerResult> Handle(Command.AddUserPreferredLocationWithStartCommand requestDto, CancellationToken cancellationToken)
    {
        var result = await currentUserService.MiniApp_AddDepartureLocationAsync(requestDto.Model);
        if (result.IsSuccess)
            await unitOfWork.SaveChangesAsync(cancellationToken);

        await currentUserService.MiniApp_AddUserPreferredLocationAsync(requestDto.Model);
        return result.ToHandlerResult();
    }
}