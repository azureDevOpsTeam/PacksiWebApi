using ApplicationLayer.CQRS.Identities.Command;
using ApplicationLayer.CQRS.Identities.Query;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(GroupName = "Identity")]
public class IdentityController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("SignIn")]
    [ProducesResponseType(typeof(SignInDto), 200)]
    public async Task<IActionResult> SignInAsync(SignInQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("SignUp")]
    public async Task<IActionResult> SignUpAsync(SignUpCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}