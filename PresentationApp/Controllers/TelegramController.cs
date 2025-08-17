using ApplicationLayer.CQRS.TelegramApis.Command;
using ApplicationLayer.CQRS.TelegramApis.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(GroupName = "Identity")]
public class TelegramController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("UserAny")]
    public async Task<IActionResult> UserAnyAsync(UserAnyQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("VerifyTelegram")]
    public async Task<IActionResult> VerifyTelegramAsync(VerifyTelegramCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}