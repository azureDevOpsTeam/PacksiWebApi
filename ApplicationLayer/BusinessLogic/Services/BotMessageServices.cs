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
            var welcomeMessage = "🎉 خوش آمدید به پکسی!\n\n" +
                "ما خیلی خوشحالیم که شما به خانواده بزرگ پکسی پیوستید! 🌟\n\n" +
                "با ربات هوشمند ما می‌تونید:\n\n" +
                "✅ درخواست حمل و نقل ثبت کنید\n" +
                "✅ پیشنهادات مسافران را ببینید و مقایسه کنید\n" +
                "✅ از امکانات ویژه و امن استفاده کنید\n\n" +
                "🌍 ابتدا کشورت رو انتخاب کن تا فقط سفرهای مرتبط رو ببینی!\n\n" +
                "✈️ **مسافری؟**\n" +
                "سفرت رو ثبت کن و پیشنهادات کاربران رو بصورت خودکار دریافت کن\n" +
                "یک یا چند بار رو با بهترین قیمت انتخاب کن و راحت به مقصد برسون!\n\n" +
                "📦 **ارسال‌کننده؟**\n" +
                "امتیاز مسافرا رو بررسی کن\n" +
                "بهترین زمان پرواز و مطمئن‌ترین مسافر رو انتخاب کن\n" +
                "و با خیال راحت بارت رو به هر جای دنیا برسون! 🌎😊\n\n" +
                "💳 **پرداخت امن و پشتیبانی کامل** تضمین شده!";

            if (!string.IsNullOrEmpty(model.ReferralCode))
            {
                welcomeMessage += $"🎁 شما با کد معرف {model.ReferralCode} وارد شده‌اید و از مزایای ویژه بهره‌مند خواهید شد!\n\n";
            }

            var inlineKeyboard = new object[][]
            {
                new object[]
                {
                    new { text = "انتخاب مبدا", callback_data = "UpdateProfile" },
                    new { text = "لیست پروازها", web_app = new { url = "https://tg.packsi.net" } }
                }
            };

            var payload = new
            {
                chat_id = model.TelegramUserId,
                text = welcomeMessage,
                parse_mode = "HTML", // یا حذفش کن
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
                        ReferredByUserId = model.ReferredByUserId
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
        var welcomeMessage = "از لیست کشورهای زیر ، کشور محل سکونت را انتخاب کنید";

        var countries = await countryRepository.Query().AsNoTracking().ToListAsync();
        var inlineKeyboard = countries
            .Select((c, index) => new { c, index })
            .GroupBy(x => x.index / 3)
            .Select(g => g.Select(x => new
            {
                text = x.c.Name,
                callback_data = $"country_{x.c.Id}"
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
}