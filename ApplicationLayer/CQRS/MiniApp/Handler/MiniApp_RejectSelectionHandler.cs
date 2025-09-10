using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_RejectSelectionHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_RejectSelectionCommand, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(MiniApp_RejectSelectionCommand requestDto, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var result = await _miniAppServices.RejectSelectionAsync(requestDto.Model);
        if (result.IsFailure)
            return result.ToHandlerResult();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result.ToHandlerResult();
    }
}