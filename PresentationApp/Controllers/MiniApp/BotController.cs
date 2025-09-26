using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs.MiniApp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace PresentationApp.Controllers.MiniApp;

[Route("api/miniapp/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(GroupName = "MiniApp")]
public class BotController(IMediator mediator, ITelegramBotClient botClient, ILogger logger) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("Referral")]
    public async Task<IActionResult> ReferralPost([FromBody] Telegram.Bot.Types.Update update)
    {
        if (update.Message?.Text is { } text && text.StartsWith("/start"))
        {
            var parts = update.Message.Text.Split(' ', 2);
            var referralCode = parts.Length > 1 ? parts[1] : null;
            var tgId = update.Message.From?.Id ?? 0;

            if (!string.IsNullOrEmpty(referralCode) && tgId != 0)
            {
                RegisterReferralDto model = new() { TelegramUserId = tgId, ReferralCode = referralCode };
                await _mediator.Send(new MiniApp_RegisterReferralCommand(model));
            }
            logger.LogInformation("متد استارت با موفقیت فراخوانی شده است");
            await botClient.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "✨ به اولین و بزرگ‌ترین ربات لجستیکی خوش آمدید! ✨\r\nاینجا می‌توانید فقط با تکمیل اطلاعات خود، بدون هیچ محدودیتی از تمام امکانات هوشمند نرم‌افزار استفاده کنید.\r\n💎 همچنین با دعوت از دوستانتان، فرصت ویژه‌ای برای کسب درآمد پایدار و همیشگی خواهید داشت.\r\n🚀 همین حالا شروع کنید و از امکانات حرفه‌ای ما لذت ببرید!"
            );
        }
        return Ok();
    }
}