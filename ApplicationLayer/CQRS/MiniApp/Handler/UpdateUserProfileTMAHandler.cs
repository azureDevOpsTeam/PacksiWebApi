using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class UpdateUserProfileTMAHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices) : IRequestHandler<UpdateUserProfileTMACommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(UpdateUserProfileTMACommand request, CancellationToken cancellationToken)
    {
        var result = await _userAccountServices.MiniApp_UpdateUserProfileAsync(request.Model);
        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}