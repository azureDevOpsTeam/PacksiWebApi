using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "MiniApp")]
[AllowAnonymous]
public class MiniAppController(IMediator mediator) : ControllerBase
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

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> CreateAsync([FromForm] CreateRequestTMACommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost]
    [Route("SendConnectionCode")]
    public async Task<IActionResult> SendConnectionCodeAsync(CreateRequestTMACommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet]
    [Route("ItemType")]
    public async Task<IActionResult> ItemTypeAsync()
        => await ResultHelper.GetResultAsync(_mediator, new ItemTypeQuery());

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