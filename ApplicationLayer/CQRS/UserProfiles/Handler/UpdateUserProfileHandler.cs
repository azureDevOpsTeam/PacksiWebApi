using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.UserProfiles.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Handler;

public class UpdateUserProfileHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices) : IRequestHandler<UpdateUserProfileCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _userAccountServices.UpdateUserProfileAsync(request.Model);
        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}