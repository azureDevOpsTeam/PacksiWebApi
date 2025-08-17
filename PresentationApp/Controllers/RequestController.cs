using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.CQRS.Requests.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
[ApiExplorerSettings(GroupName = "Users")]
public class RequestController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Create")]
    public async Task<IActionResult> CreateAsync(CreateRequestCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAsync(UpdateRequestCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet("OutboundTrips")]
    public async Task<IActionResult> OutboundTripsAsync()
     => await ResultHelper.GetResultAsync(_mediator, new OutboundTripsQuery());

    [HttpGet("InboundTrips")]
    public async Task<IActionResult> InboundTripsAsync()
     => await ResultHelper.GetResultAsync(_mediator, new InboundTripsQuery());

    [HttpGet("UserRequests")]
    public async Task<IActionResult> UserRequestsAsync()
     => await ResultHelper.GetResultAsync(_mediator, new UserRequestsQuery());

    [HttpGet("MyPostedSelected")]
    public async Task<IActionResult> MyPostedSelectedAsync()
     => await ResultHelper.GetResultAsync(_mediator, new MyPostedSelectedQuery());

    [HttpGet("GetRequestById")]
    public async Task<IActionResult> GetRequestByIdAsync(GetRequestQuery model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("SelectRequest")]
    public async Task<IActionResult> SelectRequestAsync(SelectRequestCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("RejectSelection")]
    public async Task<IActionResult> RejectSelectionAsync(RejectSelectionCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("RejectByManager")]
    public async Task<IActionResult> RejectByManagerAsync(RejectByManagerCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Published")]
    public async Task<IActionResult> PublishedAsync(PublishedCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("ConfirmedBySender")]
    public async Task<IActionResult> ConfirmedBySenderAsync(ConfirmedBySenderCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("ReadyToPickup")]
    public async Task<IActionResult> ReadyToPickupAsync(ReadyToPickupCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("PickedUp")]
    public async Task<IActionResult> PickedUpAsync(PickedUpCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("InTransit")]
    public async Task<IActionResult> InTransitAsync(InTransitCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("ReadyToDeliver")]
    public async Task<IActionResult> ReadyToDeliverAsync(ReadyToDeliverCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("Delivered")]
    public async Task<IActionResult> DeliveredAsync(DeliveredCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("NotDelivered")]
    public async Task<IActionResult> NotDeliveredAsync(NotDeliveredCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);
}