using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class ItemTypeHandler(IMiniAppServices miniAppServices, ILogger<ItemTypeHandler> logger) : IRequestHandler<ItemTypeQuery, HandlerResult>
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly ILogger<ItemTypeHandler> _logger = logger;

    public async Task<HandlerResult> Handle(ItemTypeQuery request, CancellationToken cancellationToken)
    {
        var result = await _miniAppServices.ItemTypeAsync();
        return result.ToHandlerResult();
    }
}
