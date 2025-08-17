using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class MiniAppServices(IRepository<TelegramUserInformation> telegramUserRepository, ILogger<MiniAppServices> logger) : IMiniAppServices
{
    private readonly IRepository<TelegramUserInformation> _telegramUserRepository = telegramUserRepository;
    private readonly ILogger<MiniAppServices> _logger = logger;

    public async Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync(string initData, string botToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(initData))
            {
                return Result<TelegramMiniAppValidationResultDto>.ValidationFailure("داده‌های اولیه نمی‌تواند خالی باشد");
            }

            if (string.IsNullOrWhiteSpace(botToken))
            {
                return Result<TelegramMiniAppValidationResultDto>.ValidationFailure("توکن ربات نمی‌تواند خالی باشد");
            }
            
            return await Task.Run(() =>
            {
                var parsedData = ParseInitData(initData);
                if (parsedData == null)
                {
                    return Result<TelegramMiniAppValidationResultDto>.ValidationFailure("فرمت داده‌های اولیه نامعتبر است");
                }

                var isValidSignature = ValidateSignature(initData, botToken);
                if (!isValidSignature)
                {
                    return Result<TelegramMiniAppValidationResultDto>.AuthenticationFailure("امضای دیجیتال نامعتبر است");
                }

                // بررسی زمان انقضا (معمولاً 24 ساعت)
                var authDate = DateTimeOffset.FromUnixTimeSeconds(parsedData.AuthDate).DateTime;
                if (DateTime.UtcNow.Subtract(authDate).TotalHours > 24)
                {
                    return Result<TelegramMiniAppValidationResultDto>.AuthenticationFailure("داده‌های اعتبارسنجی منقضی شده‌اند");
                }

                var validationResult = new TelegramMiniAppValidationResultDto
                {
                    IsValid = true,
                    User = parsedData.User,
                    AuthDate = authDate,
                    Hash = parsedData.Hash
                };

                return Result<TelegramMiniAppValidationResultDto>.Success(validationResult);
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در اعتبارسنجی کاربر Telegram MiniApp");
            return Result<TelegramMiniAppValidationResultDto>.GeneralFailure("خطای داخلی سرور");
        }
    }

    /// <summary>
    /// استخراج اطلاعات کاربر از داده‌های Telegram MiniApp
    /// </summary>
    public async Task<Result<TelegramMiniAppUserDto>> ExtractUserInfoFromInitDataAsync(string initData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(initData))
            {
                return Result<TelegramMiniAppUserDto>.ValidationFailure("داده‌های اولیه نمی‌تواند خالی باشد");
            }

            var parsedData = ParseInitData(initData);
            if (parsedData?.User == null)
            {
                return Result<TelegramMiniAppUserDto>.NotFound("اطلاعات کاربر در داده‌های اولیه یافت نشد");
            }

            var userInfo = new TelegramMiniAppUserDto
            {
                Id = parsedData.User.Id,
                FirstName = parsedData.User.FirstName,
                LastName = parsedData.User.LastName,
                Username = parsedData.User.Username,
                LanguageCode = parsedData.User.LanguageCode,
                IsPremium = parsedData.User.IsPremium,
                PhotoUrl = parsedData.User.PhotoUrl
            };

            return await Task.FromResult(Result<TelegramMiniAppUserDto>.Success(userInfo));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در استخراج اطلاعات کاربر");
            return Result<TelegramMiniAppUserDto>.GeneralFailure("خطای داخلی سرور");
        }
    }

    /// <summary>
    /// بررسی وجود کاربر در سیستم بر اساس Telegram User ID
    /// </summary>
    public async Task<Result<TelegramInfoDto>> CheckUserExistenceAsync(long telegramUserId)
    {
        try
        {
            var telegramUser = await _telegramUserRepository.Query()
                .Include(tu => tu.UserAccount)
                .FirstOrDefaultAsync(tu => tu.TelegramUserId == telegramUserId);

            if (telegramUser == null)
            {
                var notFoundInfo = new TelegramInfoDto
                {
                    UserExists = false,
                    TelegramUserId = telegramUserId
                };
                return Result<TelegramInfoDto>.Success(notFoundInfo);
            }

            var userInfo = new TelegramInfoDto
            {
                UserExists = true,
                TelegramUserId = telegramUser.TelegramUserId,
                PhoneNumber = telegramUser.UserAccount?.PhoneNumber,
                Language = telegramUser.Language
            };

            return Result<TelegramInfoDto>.Success(userInfo);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در بررسی وجود کاربر با شناسه تلگرام {TelegramUserId}", telegramUserId);
            return Result<TelegramInfoDto>.GeneralFailure("خطای داخلی سرور");
        }
    }

    #region Private Methods

    /// <summary>
    /// پارس کردن داده‌های اولیه Telegram MiniApp
    /// </summary>
    private TelegramInitDataDto ParseInitData(string initData)
    {
        try
        {
            var parameters = HttpUtility.ParseQueryString(initData);
            var result = new TelegramInitDataDto();

            if (parameters["query_id"] != null)
                result.QueryId = parameters["query_id"];

            if (parameters["user"] != null)
            {
                var userJson = HttpUtility.UrlDecode(parameters["user"]);
                result.User = JsonConvert.DeserializeObject<TelegramMiniAppUserDto>(userJson);
            }

            if (parameters["receiver"] != null)
                result.Receiver = parameters["receiver"];

            if (parameters["chat_type"] != null)
                result.ChatType = parameters["chat_type"];

            if (parameters["chat_instance"] != null)
                result.ChatInstance = parameters["chat_instance"];

            if (parameters["start_param"] != null)
                result.StartParam = parameters["start_param"];

            if (parameters["auth_date"] != null && long.TryParse(parameters["auth_date"], out var authDate))
                result.AuthDate = authDate;

            if (parameters["hash"] != null)
                result.Hash = parameters["hash"];

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پارس کردن داده‌های اولیه Telegram MiniApp");
            return null;
        }
    }

    /// <summary>
    /// اعتبارسنجی امضای دیجیتال داده‌های Telegram MiniApp
    /// </summary>
    private bool ValidateSignature(string initData, string botToken)
    {
        try
        {
            var parameters = HttpUtility.ParseQueryString(initData);
            var hash = parameters["hash"];

            if (string.IsNullOrEmpty(hash))
                return false;

            // حذف hash از پارامترها برای محاسبه امضا
            parameters.Remove("hash");

            // مرتب‌سازی پارامترها
            var sortedParams = parameters.AllKeys
                .Where(key => !string.IsNullOrEmpty(parameters[key]))
                .OrderBy(key => key)
                .Select(key => $"{key}={parameters[key]}")
                .ToArray();

            var dataCheckString = string.Join("\n", sortedParams);

            // محاسبه کلید مخفی
            var secretKey = ComputeHmacSha256(Encoding.UTF8.GetBytes("WebAppData"), Encoding.UTF8.GetBytes(botToken));

            // محاسبه امضای مورد انتظار
            var expectedHash = ComputeHmacSha256(secretKey, Encoding.UTF8.GetBytes(dataCheckString));
            var expectedHashHex = BitConverter.ToString(expectedHash).Replace("-", "").ToLower();

            return hash.Equals(expectedHashHex, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در اعتبارسنجی امضای دیجیتال");
            return false;
        }
    }

    /// <summary>
    /// محاسبه HMAC-SHA256
    /// </summary>
    private byte[] ComputeHmacSha256(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }

    #endregion Private Methods
}