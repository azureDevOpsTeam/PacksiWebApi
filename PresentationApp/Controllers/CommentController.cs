using ApplicationLayer.CQRS.Comments.Command;
using ApplicationLayer.CQRS.Comments.Query;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User")]
[ApiExplorerSettings(GroupName = "Users")]
public class CommentController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Create")]
    public async Task<IActionResult> CreateAsync(CreateCommentCommand model)
     => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpGet("GetComments")]
    public async Task<IActionResult> GetCommentsAsync(GetPendingCommentsQuery model)
 => await ResultHelper.GetResultAsync(_mediator, model);
}