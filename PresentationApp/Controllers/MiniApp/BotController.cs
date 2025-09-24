using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs.MiniApp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace PresentationApp.Controllers.MiniApp
{
    [Route("api/miniapp/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "MiniApp")]
    public class BotController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [Route("Referral")]
        public async Task<IActionResult> ReferralPost([FromBody] Telegram.Bot.Types.Update update)
        {
            if (update.Message?.Text != null && update.Message.Text.StartsWith("/start"))
            {
                var parts = update.Message.Text.Split(' ', 2);
                var referralCode = parts.Length > 1 ? parts[1] : null;
                var tgId = update.Message.From?.Id ?? 0;

                if (!string.IsNullOrEmpty(referralCode) && tgId != 0)
                {
                    RegisterReferralDto model = new() { TelegramUserId = tgId, ReferralCode = referralCode };
                    await _mediator.Send(new MiniApp_RegisterReferralCommand(model));
                }
            }
            return Ok();
        }
    }
}