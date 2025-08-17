using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class UserRequestsHandler(IRequestServices requestServices) : IRequestHandler<UserRequestsQuery, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(UserRequestsQuery requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.UserRequestsAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}