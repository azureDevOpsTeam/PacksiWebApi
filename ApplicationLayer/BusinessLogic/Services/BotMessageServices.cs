using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class BotMessageServices(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IRepository<Country> countryRepository, HttpClient httpClient, IConfiguration configuration, ILogger<BotMessageServices> logger) : IBotMessageServices
{
    public async Task<Result<bool>> SendWelcomeMessageAsync(RegisterReferralDto model)
    {
        try
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rnd = new Random();
            string inviteCode = new([.. Enumerable.Range(0, 6).Select(_ => chars[rnd.Next(chars.Length)])]);

            var welcomeMessage = "🎉 پکسی - packsi!\n\n" +
                "لطفا توضیحات را مطالعه کنید! 🌟\n";
            if (!string.IsNullOrEmpty(model.ReferralCode))
                welcomeMessage += $"🎁 شما با کد معرف {model.ReferralCode} وارد شده‌اید و از مزایای ویژه بهره‌مند خواهید شد!\n\n";

            var inlineKeyboard = new object[][]
            {
                new object[]
                {
                    new { text = "لیست پروازها", web_app = new { url = "https://tg.packsi.net" } },
                    new { text = "انتخاب مبدا", callback_data = "SetDeparture" }
                }
            };

            var payload = new
            {
                chat_id = model.TelegramUserId,
                text = welcomeMessage,
                parse_mode = "HTML",
                reply_markup = new
                {
                    inline_keyboard = inlineKeyboard
                }
            };

            var linkUrl = $"https://api.telegram.org/bot{configuration["TelegramBot:Token"]}/sendMessage";
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(linkUrl, content);

            var respText = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var userAccount = await userAccountServices.GetUserAccountByTelegramIdAsync(model.TelegramUserId);
                if (userAccount.Value == null)
                {
                    var newUserAccount = new UserAccount
                    {
                        TelegramId = model.TelegramUserId,
                        UserName = model.UserName,
                        ReferredByUserId = model.ReferredByUserId,
                        InviteCode = inviteCode
                    };

                    var newResult = await userAccountServices.AddUserAccountAsync(newUserAccount);
                    if (newResult.IsSuccess)
                    {
                        await unitOfWork.SaveChangesAsync();
                        var newProfile = new UserProfile()
                        {
                            UserAccountId = newUserAccount.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName
                        };
                        await userAccountServices.AddProfileAsync(newProfile);
                        await unitOfWork.SaveChangesAsync();
                    }
                }
                logger.LogInformation("پیغام خوش‌آمدگویی با موفقیت به کاربر {TelegramUserId} ارسال شد", model.TelegramUserId);
                return Result<bool>.Success(true);
            }

            logger.LogInformation("Telegram API Error: {StatusCode} - {Response}", response.StatusCode, respText);
            return Result<bool>.GeneralFailure("خطا در ارسال پیغام خوش‌آمدگویی");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "خطا در ارسال پیغام خوش‌آمدگویی به کاربر {TelegramUserId}", model.TelegramUserId);
            return Result<bool>.GeneralFailure("خطا در ارسال پیغام خوش‌آمدگویی");
        }
    }

    public async Task<Result<bool>> DepartureCountriesAsync(long telegramUserId)
    {
        using var client = new HttpClient();
        var welcomeMessage = "مرحله اول\n\n" +
        "از لیست کشورهای زیر ، کشور مبدا را انتخاب کنید";

        var countries = await countryRepository.Query().AsNoTracking().ToListAsync();
        var inlineKeyboard = countries
            .Select((c, index) => new { c, index })
            .GroupBy(x => x.index / 3)
            .Select(g => g.Select(x => new
            {
                text = x.c.Name,
                callback_data = $"departure_country_{x.c.Id}"
            }).ToArray())
            .ToArray();

        var payload = new
        {
            chat_id = telegramUserId,
            text = welcomeMessage,
            parse_mode = "HTML",
            reply_markup = new
            {
                inline_keyboard = inlineKeyboard
            },
            resize_keyboard = true,
            one_time_keyboard = true
        };

        var linkUrl = $"https://api.telegram.org/bot{configuration["TelegramBot:Token"]}/sendMessage";
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(linkUrl, content);

        response.EnsureSuccessStatusCode();

        logger.LogInformation("پیغام خوش‌آمدگویی با موفقیت به کاربر {TelegramUserId} ارسال شد", telegramUserId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> StepTwoAsync(RegisterReferralDto model)
    {
        var welcomeMessage = "🎉 مرحله دوم!\n\n" +
            "در این مرحله میتوانید کشورهایی مدنظر خود را انتخاب کنید ! 🌟\n" +
            "با انتخاب این کشور ، پروازها مطابق با مبدا و مقصدهای مورد نظر شما لیست میشوند";

        var inlineKeyboard = new object[][]
        {
                new object[]
                {
                    new { text = "ادامه در برنامه ربات", web_app = new { url = "https://tg.packsi.net" } },
                    new { text = "انتخاب مقصد", callback_data = "SetDeparture" }
                }
        };

        var payload = new
        {
            chat_id = model.TelegramUserId,
            text = welcomeMessage,
            parse_mode = "HTML",
            reply_markup = new
            {
                inline_keyboard = inlineKeyboard
            }
        };

        var linkUrl = $"https://api.telegram.org/bot{configuration["TelegramBot:Token"]}/sendMessage";
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(linkUrl, content);

        var respText = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        { }
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PreferredCountriesAsync(long telegramUserId)
    {
        using var client = new HttpClient();
        var welcomeMessage = "از لیست کشورهای زیر ، کشور مورد نظر را انتخاب کنید\n\n";

        var departureCountry = await userAccountServices.GetUserAccountByTelegramIdAsync(telegramUserId);

        var preferredLocationIds = departureCountry.Value.UserPreferredLocations
            .Select(p => p.Id)
            .ToList() ?? new List<int>();

        var countries = await countryRepository.Query()
            .Where(current =>
                current.Id != departureCountry.Value.UserProfiles.FirstOrDefault().CountryOfResidenceId &&
                !preferredLocationIds.Contains(current.Id)
            )
            .AsNoTracking()
            .ToListAsync();

        var inlineKeyboard = new object[]
        {
                new []
                {
                    new { text = "ادامه در برنامه ", web_app = new { url = "https://tg.packsi.net" } }
                }
        }.Concat(countries
         .Select((c, index) => new { c, index })
         .GroupBy(x => x.index / 3)
         .Select(g => g.Select(x => new
         {
             text = x.c.Name,
             callback_data = $"preferred_country_{x.c.Id}"
         }).ToArray())).ToArray();

        var payload = new
        {
            chat_id = telegramUserId,
            text = welcomeMessage,
            parse_mode = "HTML",
            reply_markup = new
            {
                inline_keyboard = inlineKeyboard
            },
            resize_keyboard = true,
            one_time_keyboard = true
        };

        var linkUrl = $"https://api.telegram.org/bot{configuration["TelegramBot:Token"]}/sendMessage";
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(linkUrl, content);

        response.EnsureSuccessStatusCode();

        logger.LogInformation("پیغام خوش‌آمدگویی با موفقیت به کاربر {TelegramUserId} ارسال شد", telegramUserId);
        return Result<bool>.Success(true);
    }
}