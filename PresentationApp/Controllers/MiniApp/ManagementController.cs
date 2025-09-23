using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.CQRS.MiniAppDashboard.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.MiniApp
{
    [Route("api/miniapp/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "MiniApp")]
    public class ManagementController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("GetInviteCode")]
        public async Task<IActionResult> GetInviteCodeAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_GetInvitCodeQuery());

        [HttpGet("GetDashboardData")]
        public async Task<IActionResult> GetReferralCountAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_GetDashboardDataQuery());

        [HttpGet("GetMyInvitedUsers")]
        public async Task<IActionResult> GetMyInvitedUsersAsync()
            => await ResultHelper.GetResultAsync(_mediator, new GetMyInvitedUsersQuery());
    }
}