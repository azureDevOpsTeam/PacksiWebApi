using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class BotMessageServices(IConfiguration configuration, ILogger<BotMessageServices> logger) : IBotMessageServices
{
    public async Task<Result<bool>> SendWelcomeMessageAsync(long telegramUserId, string referralCode = null)
    {
        try
        {
            using var client = new HttpClient();
            var welcomeMessage = "🎉 خوش آمدید به پکسی!\n\n" +
                               "ما خوشحالیم که شما به خانواده پکسی پیوستید. " +
                               "با استفاده از ربات ما می‌توانید:\n\n" +
                               "✅ درخواست حمل و نقل ثبت کنید\n" +
                               "✅ پیشنهادات مسافران را مشاهده کنید\n" +
                               "✅ از امکانات ویژه استفاده کنید\n\n";

            if (!string.IsNullOrEmpty(referralCode))
            {
                welcomeMessage += $"🎁 شما با کد معرف {referralCode} وارد شده‌اید و از مزایای ویژه بهره‌مند خواهید شد!\n\n";
            }

            var payload = new
            {
                chat_id = telegramUserId,
                text = welcomeMessage,
                parse_mode = "HTML",
                //reply_markup = new
                //{
                //    inline_keyboard = new[]
                //    {
                //        new[]
                //        {
                //            new { text = "تایید شماره موبایل", callback_data = "confirmPhoneNumber" },
                //            new { text = "تکمیل پروفایل", callback_data = "UpdateProfile" }
                //        },
                //        [
                //            new { text = "لیست سفرها و پیشنهادات", callback_data = "OpenWebApp" }
                //        ]
                //    }
                //}
            };

            var linkUrl = $"https://api.telegram.org/bot{configuration["TelegramBot:Token"]}/sendMessage";
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(linkUrl, content
            );

            response.EnsureSuccessStatusCode();

            logger.LogInformation("پیغام خوش‌آمدگویی با موفقیت به کاربر {TelegramUserId} ارسال شد", telegramUserId);
            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "خطا در ارسال پیغام خوش‌آمدگویی به کاربر {TelegramUserId}", telegramUserId);
            return Result<bool>.GeneralFailure("خطا در ارسال پیغام خوش‌آمدگویی");
        }
    }
}