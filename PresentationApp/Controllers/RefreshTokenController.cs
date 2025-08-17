using ApplicationLayer.CQRS.RefreshTokens.Command;
using ApplicationLayer.CQRS.RefreshTokens.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "Identity")]
public class RefreshTokenController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> RefreshToken(TokenRequestQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("RevokeRefreshToken")]
    public async Task<IActionResult> RevokeRefreshToken(RevokeRefreshTokenCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}