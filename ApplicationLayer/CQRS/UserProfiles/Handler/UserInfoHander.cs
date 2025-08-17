using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.UserProfiles.Query;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Handler;

public class UserInfoHander(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices) : IRequestHandler<UserInfoQuery, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(UserInfoQuery request, CancellationToken cancellationToken)
    {
        var result = await _userAccountServices.UserInfoAsync();
        if (result.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };

        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}