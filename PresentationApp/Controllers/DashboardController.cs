using ApplicationLayer.CQRS.Dashboard.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
[ApiExplorerSettings(GroupName = "Users")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("ReportTrips")]
    public async Task<IActionResult> ReportTripsAsync()
     => await ResultHelper.GetResultAsync(_mediator, new ReportTripsQuery());

    [HttpGet("InboundOutboundTrips")]
    public async Task<IActionResult> InboundOutboundTripsAsync()
     => await ResultHelper.GetResultAsync(_mediator, new InboundOutboundTripsQuery());
}