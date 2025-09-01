using ApplicationLayer.CQRS.Regions.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.MiniApp;

[Route("api/miniapp/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "MiniApp")]
public class RegionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    [Route("GetAllPhonePrefix")]
    public async Task<IActionResult> GetAllPhonePrefixAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetPhonePrefixDropdownQuery());

    [HttpGet]
    [AllowAnonymous]
    [Route("GetCountries")]
    public async Task<IActionResult> GetCountriesAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetCountriesQuery());

    [HttpPost]
    [AllowAnonymous]
    [Route("GetCities")]
    public async Task<IActionResult> GetCitiesAsync(GetCitiesQuery model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet]
    [AllowAnonymous]
    [Route("GetCitiesTree")]
    public async Task<IActionResult> GetCitiesTreeAsync()
       => await ResultHelper.GetResultAsync(_mediator, new GetCitiesTreeQuery());
}