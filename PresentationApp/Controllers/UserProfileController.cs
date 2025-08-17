using ApplicationLayer.CQRS.UserProfiles.Command;
using ApplicationLayer.CQRS.UserProfiles.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator, User")]
[ApiExplorerSettings(GroupName = "Users")]
public class UserProfileController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("UserInfo")]
    public async Task<IActionResult> UserInfoAsync()
     => await ResultHelper.GetResultAsync(_mediator, new UserInfoQuery());

    [HttpPost("AddUserPreferredLocation")]
    public async Task<IActionResult> AddUserPreferredLocationAsync(AddUserPreferredLocationCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("UpdateUserProfile")]
    public async Task<IActionResult> UpdateUserProfileAsync(UpdateUserProfileCommand model)
    => await ResultHelper.GetResultAsync(_mediator, model);
}