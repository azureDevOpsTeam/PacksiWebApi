using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class MiniAppServices(IRepository<TelegramUserInformation> telegramUserRepository, IRepository<UserAccount> userAccountRepository, IRepository<UserProfile> userProfileRepository, IRepository<UserPreferredLocation> userPreferredLocation, IRepository<Request> requestRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<MiniAppServices> logger) : IMiniAppServices
{
    private readonly IRepository<TelegramUserInformation> _telegramUserRepository = telegramUserRepository;
    private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
    private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepository<UserPreferredLocation> _userPreferredLocation = userPreferredLocation;
    private readonly IRepository<Request> _requestRepository = requestRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<MiniAppServices> _logger = logger;
    //private readonly TelegramBotClient _botClient = new(configuration["TelegramBot:Token"] ?? throw new InvalidOperationException("TelegramBot:Token configuration is missing"));

    public async Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync()
    {
        try
        {
            var botToken = "8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A";
            var initData = _httpContextAccessor.HttpContext?.Request.Headers["X-Telegram-Init-Data"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(initData) || string.IsNullOrWhiteSpace(botToken))
            {
                _logger.LogWarning("InitData is missing or empty");
                return Result<TelegramMiniAppValidationResultDto>.ValidationFailure("اطلاعات نامعتبر");
            }

            var parsedData = ParseInitData(initData);
            if (parsedData == null)
                return Result<TelegramMiniAppValidationResultDto>.ValidationFailure("فرمت داده‌های اولیه نامعتبر است");

            var isValidSignature = ValidateSignature(initData, botToken);
            if (!isValidSignature)
                return Result<TelegramMiniAppValidationResultDto>.AuthenticationFailure("امضای دیجیتال نامعتبر است");

            var authDate = DateTimeOffset.FromUnixTimeSeconds(parsedData.AuthDate).DateTime;
            if (DateTime.UtcNow.Subtract(authDate).TotalHours > 24)
                return Result<TelegramMiniAppValidationResultDto>.AuthenticationFailure("داده‌های اعتبارسنجی منقضی شده‌اند");

            var validationResult = new TelegramMiniAppValidationResultDto
            {
                IsValid = true,
                User = parsedData.User,
                AuthDate = authDate,
                Hash = parsedData.Hash,
                ExistUser = await _userAccountRepository.GetDbSet().AnyAsync(current => current.TelegramId == parsedData.User.Id)
            };

            return Result<TelegramMiniAppValidationResultDto>.Success(validationResult);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در اعتبارسنجی کاربر");
            return Result<TelegramMiniAppValidationResultDto>.GeneralFailure("خطای داخلی سرور");
        }
    }

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

    public async Task<Result<List<OutboundDto>>> OutboundTripsAsync(UserAccount user)
    {
        try
        {
            var userCountryId = await _userProfileRepository.Query()
                .Where(p => p.UserAccountId == user.Id)
                .Select(p => p.CountryOfResidenceId)
                .FirstOrDefaultAsync();

            var preferredCountryIds = await _userPreferredLocation.Query()
                .Where(p => p.UserAccountId == user.Id && p.CountryId != null)
                .Select(p => p.CountryId.Value)
                .ToListAsync();

            var outboundRequests = await _requestRepository.Query()
                .Include(current => current.RequestItemTypes)
                .Where(r =>
                    r.OriginCity != null &&
                    r.OriginCity.CountryId == userCountryId &&
                    r.DestinationCity != null &&
                    preferredCountryIds.Contains(r.DestinationCity.CountryId))
                .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                .Select(current => new OutboundDto
                {
                    RequestId = current.Id,
                    UserAccountId = current.UserAccountId,
                    ArrivalDate = current.ArrivalDate,
                    DepartureDate = current.DepartureDate,
                    ArrivalDatePersian = DateTimeHelper.GetPersianDate(current.ArrivalDate),
                    DepartureDatePersian = DateTimeHelper.GetPersianDate(current.DepartureDate),
                    Description = current.Description,
                    DestinationCity = current.DestinationCity.Name,
                    DestinationCityFa = current.DestinationCity.PersianName,
                    OriginCity = current.OriginCity.Name,
                    OriginCityFa = current.OriginCity.PersianName,
                    ItemTypes = current.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).Name).ToArray(),
                    ItemTypesFa = current.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).PersianName).ToArray(),
                    MaxHeightCm = current.MaxHeightCm,
                    MaxLengthCm = current.MaxLengthCm,
                    MaxWeightKg = current.MaxWeightKg,
                    MaxWidthCm = current.MaxWidthCm
                }).ToListAsync();

            return Result<List<OutboundDto>>.Success(outboundRequests);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت پروازهای خروجی {UserId}", user.Id);
            return Result<List<OutboundDto>>.GeneralFailure("خطا در دریافت پروازهای خروجی");
        }
    }

    public async Task<Result<List<OutboundDto>>> InboundTripsQueryAsync(UserAccount user)
    {
        try
        {
            var userCountryId = await _userProfileRepository.Query()
               .Where(p => p.UserAccountId == user.Id)
               .Select(p => p.CountryOfResidenceId)
               .FirstOrDefaultAsync();

            var preferredCountryIds = await _userPreferredLocation.Query()
                .Where(p => p.UserAccountId == user.Id && p.CountryId != null)
                .Select(p => p.CountryId.Value)
                .ToListAsync();

            var inboundRequests = await _requestRepository.Query()
                .Where(r =>
                    r.OriginCity != null &&
                    preferredCountryIds.Contains(r.OriginCity.CountryId) &&
                    r.DestinationCity != null &&
                    r.DestinationCity.CountryId == userCountryId)
                .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                .Select(current => new OutboundDto
                {
                    RequestId = current.Id,
                    UserAccountId = current.UserAccountId,
                    ArrivalDate = current.ArrivalDate,
                    DepartureDate = current.DepartureDate,
                    ArrivalDatePersian = DateTimeHelper.GetPersianDate(current.ArrivalDate),
                    DepartureDatePersian = DateTimeHelper.GetPersianDate(current.DepartureDate),
                    Description = current.Description,
                    DestinationCity = current.DestinationCity.Name,
                    DestinationCityFa = current.DestinationCity.PersianName,
                    OriginCity = current.OriginCity.Name,
                    OriginCityFa = current.OriginCity.PersianName,
                    ItemTypes = current.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.Id).Name).ToArray(),
                    ItemTypesFa = current.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.Id).PersianName).ToArray(),
                    MaxHeightCm = current.MaxHeightCm,
                    MaxLengthCm = current.MaxLengthCm,
                    MaxWeightKg = current.MaxWeightKg,
                    MaxWidthCm = current.MaxWidthCm
                })
                .ToListAsync();

            return Result<List<OutboundDto>>.Success(inboundRequests);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت پروازهای ورودی {UserId}", user.Id);
            return Result<List<OutboundDto>>.GeneralFailure("خطا در دریافت پروازهای ورودی");
        }
    }

    public async Task<Result<bool>> SendMessageAsync(long chatId)
    {
        try
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            //var sentMessage = await _botClient.SendMessage(
            //    chatId: chatId,
            //    text: code
            //);

            _logger.LogInformation("پیام با موفقیت به چت {ChatId} ارسال شد", chatId);
            return Result<bool>.Success(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ارسال پیام به چت {ChatId}", chatId);
            return Result<bool>.GeneralFailure("خطا در ارسال پیام");
        }
    }

    public Task<Result<List<RequestItemTypeDto>>> ItemTypeAsync()
    {
        try
        {
            var result = TransportableItemTypeEnum.List
                .Select(current => new RequestItemTypeDto
                {
                    ItemTypeId = current.Value,
                    ItemType = current.Name,
                    PersianName = current.PersianName,
                })
                .ToList();

            return Task.FromResult(Result<List<RequestItemTypeDto>>.Success(result));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت لیست آیتم‌ها");
            return Task.FromResult(Result<List<RequestItemTypeDto>>.GeneralFailure("خطا در دریافت لیست آیتم‌ها"));
        }
    }

    #region Private Methods

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

    private byte[] ComputeHmacSha256(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }

    #endregion Private Methods
}