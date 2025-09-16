using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class MiniApp_CreateRequestHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IMiniAppServices miniAppServices) : IRequestHandler<MiniApp_CreateRequestCommand, HandlerResult>
{
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(MiniApp_CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var resultValidation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (resultValidation.IsFailure)
            return resultValidation.ToHandlerResult();

        var userTelegramInfo = resultValidation.Value.User;
        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(userTelegramInfo.Id);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var resultAddRequest = await _miniAppServices.AddRequestAsync(request, userAccount.Value, cancellationToken);

            if (resultAddRequest.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return resultAddRequest.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var resultItemtype = await _miniAppServices.AddRequestItemTypeAsync(request, resultAddRequest.Value.Id);
            if (resultItemtype.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return resultItemtype.ToHandlerResult();
            }

            var resultAttachment = await _miniAppServices.AddRequestAttachmentAsync(resultAddRequest.Value.Id, request.Files, RequestTypeEnum.FromValue(request.RequestType));
            if (resultAttachment.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return resultAttachment.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitAsync();

            return resultItemtype.ToHandlerResult();
        }
        catch (Exception exception)
        {
            await _unitOfWork.RollbackAsync();
            return new HandlerResult() { ObjectResult = RequestStatus.Failed, Message = "خطا در ثبت درخواست" };
        }
    }
}