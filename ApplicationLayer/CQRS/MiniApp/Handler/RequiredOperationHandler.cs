using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class RequiredOperationHandler(IUserAccountServices userAccountServices) : IRequestHandler<RequiredOperationQuery, HandlerResult>
{
    private readonly IUserAccountServices _userAccountServices = userAccountServices;

    public async Task<HandlerResult> Handle(RequiredOperationQuery request, CancellationToken cancellationToken)
    {
        var result = await _userAccountServices.MiniApp_RequiredOperationAsync();
        if (result.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };

        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}