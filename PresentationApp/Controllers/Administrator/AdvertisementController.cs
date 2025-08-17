using ApplicationLayer.CQRS.Advertisements.Command;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.Administrator;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
[ApiExplorerSettings(GroupName = "Administrator")]
public class AdvertisementController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Accept")]
    public async Task<IActionResult> AcceptAsync(AcceptCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Publish")]
    public async Task<IActionResult> PublishAsync(PublishCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Reject")]
    public async Task<IActionResult> RejectAsync(RejectCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}