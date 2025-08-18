using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Users")]
[AllowAnonymous]
public class MiniAppController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [Route("validate")]
    public async Task<IActionResult> ValidateUserAsync(UserValicationQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAsync(CreateRequestTMACommand model)
    => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("SendConnectionCode")]
    public async Task<IActionResult> SendConnectionCodeAsync(CreateRequestTMACommand model)
    => await ResultHelper.GetResultAsync(_mediator, model);
}