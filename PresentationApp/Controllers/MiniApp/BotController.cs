using ApplicationLayer.CQRS.MiniApp.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Post([FromBody] Telegram.Bot.Types.Update update)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(update));

            //if (update.Message?.Text != null && update.Message.Text.StartsWith("/start"))
            //{
            //    var parts = update.Message.Text.Split(' ', 2);
            //    var referralCode = parts.Length > 1 ? parts[1] : null;
            //    var tgId = update.Message.From?.Id ?? 0;
            //
            //    if (!string.IsNullOrEmpty(referralCode) && tgId != 0)
            //    {
            //        await _mediator.Send(new MiniApp_RegisterReferralCommand(tgId, referralCode));
            //    }
            //}
            return Ok();
        }
    }
}