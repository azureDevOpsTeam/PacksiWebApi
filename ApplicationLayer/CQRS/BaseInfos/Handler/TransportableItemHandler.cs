using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.BaseInfos.Query;
using MediatR;

namespace ApplicationLayer.CQRS.BaseInfos.Handler;

public class TransportableItemHandler(IBaseInfoService baseInfoService) : IRequestHandler<TransportableItemQuery, HandlerResult>
{
    private readonly IBaseInfoService _baseInfoService = baseInfoService;

    public async Task<HandlerResult> Handle(TransportableItemQuery request, CancellationToken cancellationToken)
    {
        var result = await _baseInfoService.TransportableItemAsync();
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}