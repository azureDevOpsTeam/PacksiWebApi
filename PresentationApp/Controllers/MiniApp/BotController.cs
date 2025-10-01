using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace PresentationApp.Controllers.MiniApp;

[Route("api/miniapp/[controller]")]
[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(GroupName = "MiniApp")]
public class BotController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Route("Start")]
    public async Task<IActionResult> StartPost([FromBody] Update update)
    {
        if (update.Message?.Text != null && update.Message.Text.StartsWith("/start"))
        {
            var parts = update.Message.Text.Split(' ', 2);
            var referralCode = parts.Length > 1 ? parts[1] : null;
            var tgId = update.Message.From?.Id ?? 0;
            var username = update.Message.From?.Username ?? null;
            var firstName = update.Message.From?.FirstName ?? null;
            var lastName = update.Message.From?.LastName ?? null;

            RegisterReferralDto model = new() { TelegramUserId = tgId, ReferralCode = referralCode };

            if (!string.IsNullOrEmpty(referralCode) && tgId != 0)
            {
                await _mediator.Send(new MiniApp_RegisterReferralCommand(model));
            }
            else
                await _mediator.Send(new MiniApp_SendMessageToUserCommand(model));
        }
        else if (update.CallbackQuery != null)
        {
            var callbackData = update.CallbackQuery.Data;
            var telegramUserId = update.CallbackQuery.From.Id;

            switch (callbackData)
            {
                case "SetDeparture":
                    await _mediator.Send(new MiniApp_DepartureCountriesCommand(telegramUserId));
                    break;
                case "SetPreferred":
                    await _mediator.Send(new MiniApp_DepartureCountriesCommand(telegramUserId));
                    break;

                default:
                    if (callbackData.StartsWith("departure_country_"))
                    {
                        var countryId = int.Parse(callbackData.Replace("departure_country_", ""));
                        CountryOfResidenceDto locationDto = new() { TelegramId = telegramUserId, CountryOfResidenceId = countryId };
                        await _mediator.Send(new AddUserPreferredLocationWithStartCommand(locationDto));
                    }
                    else if (callbackData.StartsWith("preferred_country_"))
                    {
                        var countryId = int.Parse(callbackData.Replace("departure_country_", ""));
                        CountryOfResidenceDto locationDto = new() { TelegramId = telegramUserId, CountryOfResidenceId = countryId };
                        await _mediator.Send(new AddUserPreferredLocationWithStartCommand(locationDto));
                    }
                    break;
            }
        }
        return Ok();
    }
}