using ApplicationLayer.CQRS.Advertisements.Command;
using ApplicationLayer.CQRS.Advertisements.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User, Administrator")]
[ApiExplorerSettings(GroupName = "Users")]
public class AdvertisementController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Create")]
    public async Task<IActionResult> CreateAsync(CreateCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllAsync(GetAllQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Get")]
    public async Task<IActionResult> GetAsync(GetByIdQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteAsync(DeleteCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);
}