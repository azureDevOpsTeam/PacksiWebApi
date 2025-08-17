using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Requests.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Requests.Handler;

public class MyPostedSelectedHandler(IRequestServices requestServices) : IRequestHandler<MyPostedSelectedQuery, HandlerResult>
{
    private readonly IRequestServices _requestServices = requestServices;

    public async Task<HandlerResult> Handle(MyPostedSelectedQuery requestDto, CancellationToken cancellationToken)
    {
        var result = await _requestServices.MyPostedSelectedAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}