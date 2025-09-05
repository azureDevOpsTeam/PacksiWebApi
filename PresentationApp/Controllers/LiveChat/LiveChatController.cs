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

    /// <summary>
    /// دریافت لیست کاربران برای شروع چت
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsersListAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetUsersListQuery());

    /// <summary>
    /// دریافت مکالمات اخیر کاربر
    /// </summary>
    /// <returns></returns>
    [HttpGet("conversations")]
    public async Task<IActionResult> GetRecentConversationsAsync()
        => await ResultHelper.GetResultAsync(_mediator, new GetRecentConversationsQuery());

    /// <summary>
    /// دریافت پیام‌های یک مکالمه
    /// </summary>
    /// <param name="conversationId">شناسه مکالمه</param>
    /// <param name="page">شماره صفحه</param>
    /// <param name="pageSize">تعداد پیام در هر صفحه</param>
    /// <returns></returns>
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetConversationMessagesAsync(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        => await ResultHelper.GetResultAsync(_mediator, new GetConversationMessagesQuery(conversationId, page, pageSize));

    /// <summary>
    /// ارسال پیام جدید
    /// </summary>
    /// <param name="model">اطلاعات پیام</param>
    /// <returns></returns>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageDto model)
        => await ResultHelper.GetResultAsync(_mediator, new SendMessageCommand(model));

    /// <summary>
    /// مسدود کردن کاربر
    /// </summary>
    /// <param name="model">اطلاعات مسدودسازی</param>
    /// <returns></returns>
    [HttpPost("block")]
    public async Task<IActionResult> BlockUserAsync([FromBody] BlockUserDto model)
        => await ResultHelper.GetResultAsync(_mediator, new BlockUserCommand(model));

    /// <summary>
    /// علامت‌گذاری پیام‌ها به عنوان خوانده شده
    /// </summary>
    /// <param name="conversationId">شناسه مکالمه</param>
    /// <returns></returns>
    [HttpPut("conversations/{conversationId}/mark-read")]
    public async Task<IActionResult> MarkMessagesAsReadAsync(int conversationId)
        => await ResultHelper.GetResultAsync(_mediator, new MarkMessagesAsReadCommand(conversationId));
}