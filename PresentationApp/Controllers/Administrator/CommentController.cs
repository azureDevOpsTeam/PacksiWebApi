using ApplicationLayer.CQRS.Comments.Command;
using ApplicationLayer.CQRS.Comments.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.Administrator;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
[ApiExplorerSettings(GroupName = "Administrator")]
public class CommentController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPut("Pending")]
    public async Task<IActionResult> GetPendingCommentsAsync(GetPendingCommentsQuery model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet("GetById")]
    public async Task<IActionResult> GetByIdAsync(GetCommentByIdQuery model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPut("Approve")]
    public async Task<IActionResult> ApproveAsync(ApproveCommentCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);
}