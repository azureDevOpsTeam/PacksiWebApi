using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.MiniApp.Query;
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
    public class IdentityController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [Route("validate")]
        public async Task<IActionResult> ValidateUserAsync()
            => await ResultHelper.GetResultAsync(_mediator, new UserValicationQuery());

        [HttpPost]
        [Route("VerifyPhoneNumber")]
        public async Task<IActionResult> VerifyPhoneNumberAsync(VerifyPhoneNumberTMACommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpGet]
        [Route("UserInfo")]
        public async Task<IActionResult> UserInfoAsync()
            => await ResultHelper.GetResultAsync(_mediator, new UserInfoTMAQuery());

        [HttpPost("AddUserPreferredLocation")]
        public async Task<IActionResult> AddUserPreferredLocationAsync(AddUserPreferredLocationTMACommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileAsync(UpdateUserProfileTMACommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);
    }
}
