using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.MiniApp;

[Route("api/miniapp/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(GroupName = "MiniApp")]
public class PaymentController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("PaySuggestedPrice")]
    public async Task<IActionResult> PaySuggestedPriceAsync(MiniApp_PaySuggestedPriceQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}