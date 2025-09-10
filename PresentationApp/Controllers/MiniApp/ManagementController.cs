using ApplicationLayer.CQRS.Managers.Command;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.MiniApp
{
    [Route("api/miniapp/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    [ApiExplorerSettings(GroupName = "Managers")]
    public class ManagementController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("InviteUser")]
        public async Task<IActionResult> InviteUserAsync(InviteUserCommand command)
        => await ResultHelper.GetResultAsync(_mediator, command);
    }
}