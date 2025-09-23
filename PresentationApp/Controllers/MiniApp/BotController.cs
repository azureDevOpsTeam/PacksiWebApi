using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs.MiniApp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace PresentationApp.Controllers.MiniApp
{
    [Route("api/miniapp/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "MiniApp")]
    public class BotController(IMediator mediator, ITelegramBotClient botClient) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ITelegramBotClient _botClient = botClient;

        private static readonly List<(int Id, string Name)> Cities = new()
{
    (1, "تهران"),
    (2, "مشهد"),
    (3, "اصفهان"),
    (4, "شیراز"),
    (5, "تبریز"),
};

        // حالت‌های گفتگو
        public enum ConversationState
        {
            None,
            WaitingForName,
            WaitingForOriginCity,
            WaitingForDestinationCity,
            Completed
        }

        // ذخیره وضعیت کاربر
        private static Dictionary<long, ConversationState> _userStates = new();

        private static Dictionary<long, UserFormData> _userForms = new();

        public class UserFormData
        {
            public string Name { get; set; }

            public int OriginCityId { get; set; }

            public int DestinationCityId { get; set; }
        }

        [HttpPost]
        [Route("Referral")]
        public async Task<IActionResult> ReferralPost([FromBody] Telegram.Bot.Types.Update update)
        {
            var tgId = update.Message.From?.Id ?? 0;
            if (update.Message?.Text != null && update.Message.Text.StartsWith("/start"))
            {
                var parts = update.Message.Text.Split(' ', 2);
                var referralCode = parts.Length > 1 ? parts[1] : null;

                if (!string.IsNullOrEmpty(referralCode) && tgId != 0)
                {
                    RegisterReferralDto model = new() { TelegramUserId = tgId, ReferralCode = referralCode };
                    await _mediator.Send(new MiniApp_RegisterReferralCommand(model));
                }
            }
            else if (update.Message?.Text != null && _userStates.TryGetValue(tgId, out var state))
            {
                if (state == ConversationState.WaitingForName)
                {
                    _userForms[tgId].Name = update.Message.Text;
                    _userStates[tgId] = ConversationState.WaitingForOriginCity;

                    // ساخت کیبورد برای شهر مبدا
                    var keyboard = new InlineKeyboardMarkup(
                        Cities.Select(c => InlineKeyboardButton.WithCallbackData(c.Name, $"origin:{c.Id}")).Chunk(2)
                    );

                    await _botClient.SendMessage(
                        chatId: tgId,
                        text: "✅ نام ثبت شد. حالا شهر مبدا را انتخاب کنید:",
                        replyMarkup: keyboard
                    );
                }
            }
            // انتخاب مبدا یا مقصد
            else if (update.CallbackQuery != null)
            {
                var data = update.CallbackQuery.Data;

                if (data.StartsWith("origin:"))
                {
                    var cityId = int.Parse(data.Split(':')[1]);
                    _userForms[tgId].OriginCityId = cityId;
                    _userStates[tgId] = ConversationState.WaitingForDestinationCity;

                    var keyboard = new InlineKeyboardMarkup(
                        Cities.Select(c => InlineKeyboardButton.WithCallbackData(c.Name, $"dest:{c.Id}")).Chunk(2)
                    );

                    await _botClient.SendMessage(
                        chatId: tgId,
                        text: "✅ مبدا ثبت شد. حالا شهر مقصد را انتخاب کنید:",
                        replyMarkup: keyboard
                    );
                }
                else if (data.StartsWith("dest:"))
                {
                    var cityId = int.Parse(data.Split(':')[1]);
                    _userForms[tgId].DestinationCityId = cityId;
                    _userStates[tgId] = ConversationState.Completed;

                    var form = _userForms[tgId];

                    await _botClient.SendMessage(
                        chatId: tgId,
                        text: $"🎉 فرم تکمیل شد!\n" +
                              $"نام: {form.Name}\n" +
                              $"مبدا: {Cities.First(c => c.Id == form.OriginCityId).Name}\n" +
                              $"مقصد: {Cities.First(c => c.Id == form.DestinationCityId).Name}"
                    );
                }
            }
            return Ok();
        }
    }
}