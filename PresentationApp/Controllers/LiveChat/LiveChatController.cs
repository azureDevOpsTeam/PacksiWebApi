using ApplicationLayer.CQRS.LiveChat.Command;
using ApplicationLayer.CQRS.LiveChat.Query;
using ApplicationLayer.DTOs.LiveChat;
using ApplicationLayer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PresentationApp.Controllers.LiveChat;

[Route("api/miniapp/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "MiniApp")]
public class LiveChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public LiveChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsersListAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetUsersListQuery());

    [HttpGet("conversations")]
    public async Task<IActionResult> GetRecentConversationsAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetRecentConversationsQuery());

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetConversationMessagesAsync(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => await ResultHelper.GetResultAsync(_mediator, new GetConversationMessagesQuery(conversationId, page, pageSize));

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessageAsync(SendMessageCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPost("block")]
    public async Task<IActionResult> BlockUserAsync(BlockUserCommand model)
        => await ResultHelper.GetResultAsync(_mediator, model);

    [HttpPut("conversations/{conversationId}/mark-read")]
    public async Task<IActionResult> MarkMessagesAsReadAsync(int conversationId)
        => await ResultHelper.GetResultAsync(_mediator, new MarkMessagesAsReadCommand(conversationId));
}