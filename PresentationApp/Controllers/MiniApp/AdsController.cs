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
public class AdsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Route("GetAds")]
    public async Task<IActionResult> GetAdsAsync()
        => await ResultHelper.GetResultAsync(_mediator, new MiniApp_GetAdsQuery());
}