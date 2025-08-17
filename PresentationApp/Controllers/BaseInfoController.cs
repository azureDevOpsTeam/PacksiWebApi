using ApplicationLayer.CQRS.BaseInfos.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
[ApiExplorerSettings(GroupName = "Users")]
public class BaseInfoController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    [Route("TransportableItem")]
    public async Task<IActionResult> TransportableItemAsync()
        => await ResultHelper.GetResultAsync(_mediator, new TransportableItemQuery());
}