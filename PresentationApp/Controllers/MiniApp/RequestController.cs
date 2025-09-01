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
    public class RequestController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAsync([FromForm] MiniApp_CreateRequestCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpGet]
        [Route("ItemType")]
        public async Task<IActionResult> ItemTypeAsync()
            => await ResultHelper.GetResultAsync(_mediator, new ItemTypeQuery());

        [HttpGet("OutboundTrips")]
        public async Task<IActionResult> OutboundTripsAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_OutboundTripsQuery());

        [HttpGet("InboundTrips")]
        public async Task<IActionResult> InboundTripsAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_InboundTripsQuery());
    }
}