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

        [HttpGet("GetRequestTrips")]
        public async Task<IActionResult> GetRequestTripsAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_GetRequestTripsQuery());

        [HttpGet("GetMyRequestTrips")]
        public async Task<IActionResult> GetMyRequestTripsAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_GetMyRequestTripsQuery());

        //[HttpGet("InboundTrips")]
        //public async Task<IActionResult> InboundTripsAsync()
        //    => await ResultHelper.GetResultAsync(_mediator, new MiniApp_InboundTripsQuery());

        [HttpGet("UserRequests")]
        public async Task<IActionResult> UserRequestsAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_UserRequestsQuery());

        [HttpGet("MyPostedSelected")]
        public async Task<IActionResult> MyPostedSelectedAsync()
            => await ResultHelper.GetResultAsync(_mediator, new MiniApp_MyPostedSelectedQuery());

        [HttpPost("GetRequestById")]
        public async Task<IActionResult> GetRequestByIdAsync(MiniApp_GetRequestQuery model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("SelectRequest")]
        public async Task<IActionResult> SelectRequestAsync([FromForm] MiniApp_CreateSuggestionCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        //Changes....
        [HttpPost("RejectSelection")]
        public async Task<IActionResult> RejectSelectionAsync(MiniApp_RejectSelectionCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("RejectByManager")]
        public async Task<IActionResult> RejectByManagerAsync(MiniApp_RejectByManagerCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("Published")]
        public async Task<IActionResult> PublishedAsync(MiniApp_PublishedCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("ConfirmedBySender")]
        public async Task<IActionResult> ConfirmedBySenderAsync(MiniApp_ConfirmedBySenderCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("ReadyToPickup")]
        public async Task<IActionResult> ReadyToPickupAsync(MiniApp_ReadyToPickupCommand model)
            => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("PickedUp")]
        public async Task<IActionResult> PickedUpAsync(MiniApp_PickedUpCommand model)
         => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("InTransit")]
        public async Task<IActionResult> InTransitAsync(MiniApp_InTransitCommand model)
         => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("ReadyToDeliver")]
        public async Task<IActionResult> ReadyToDeliverAsync(MiniApp_ReadyToDeliverCommand model)
         => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("Delivered")]
        public async Task<IActionResult> DeliveredAsync(MiniApp_DeliveredCommand model)
         => await ResultHelper.GetResultAsync(_mediator, model);

        [HttpPost("NotDelivered")]
        public async Task<IActionResult> NotDeliveredAsync(MiniApp_NotDeliveredCommand model)
         => await ResultHelper.GetResultAsync(_mediator, model);
    }
}