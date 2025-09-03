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
public class MiniAppServices(IRepository<TelegramUserInformation> telegramUserRepository, IRepository<UserAccount> userAccountRepository,
    IRepository<UserProfile> userProfileRepository, IRepository<UserPreferredLocation> userPreferredLocation,
    IRepository<Request> requestRepository, IRepository<RequestItemType> itemTypeRepo, IRepository<RequestSelection> requestSelection,
    IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<MiniAppServices> logger) : IMiniAppServices
{
    private readonly IRepository<TelegramUserInformation> _telegramUserRepository = telegramUserRepository;
    private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
    private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepository<UserPreferredLocation> _userPreferredLocation = userPreferredLocation;
    private readonly IRepository<Request> _requestRepository = requestRepository;
    private readonly IRepository<RequestItemType> _itemTypeRepo = itemTypeRepo;
    private readonly IRepository<RequestSelection> _requestSelection = requestSelection;
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<MiniAppServices> _logger = logger;
    //private readonly TelegramBotClient _botClient = new(configuration["TelegramBot:Token"] ?? throw new InvalidOperationException("TelegramBot:Token configuration is missing"));

    public async Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync()
    {
        try
        {
            var botToken = "8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A";
            //var initData = _httpContextAccessor.HttpContext?.Request.Headers["X-Telegram-Init-Data"].FirstOrDefault();
            var initData = "user=%7B%22id%22%3A5933914644%2C%22first_name%22%3A%22Shahram%22%2C%22last_name%22%3A%22%22%2C%22username%22%3A%22ShahramOweisy%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%2C%22photo_url%22%3A%22https%3A%5C%2F%5C%2Ft.me%5C%2Fi%5C%2Fuserpic%5C%2F320%5C%2FQGwtYapyXkY4-jZJkczPeUb_XKfimJozOKy8lZzBhtQc4cO4xBQzwdPwcb_QSNih.svg%22%7D&chat_instance=-2675852455221065738&chat_type=sender&auth_date=1756919843&signature=oVTvDp3bgZDaX_Yds0PCzANzn1HuH6xTGfLaR3WXKzzwU8e2kceGTS79nkv9Jugd0JYxT5CxBkTyWtD0kd55Bw&hash=f5ca10f64ee5cc692049905c26cadb73fdd05c22bee2fcb1b71da41a149f3f14";
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
                .Where(r =>
                    r.OriginCity != null &&
                    r.OriginCity.CountryId == userCountryId &&
                    r.DestinationCity != null &&
                    preferredCountryIds.Contains(r.DestinationCity.CountryId))
                .Include(current => current.UserAccount)
                    .ThenInclude(current => current.UserProfiles)
                .Include(current => current.StatusHistories)
                .Include(current => current.RequestItemTypes)
                .Include(current => current.OriginCity)
                    .ThenInclude(current => current.Country)
                .Include(current => current.DestinationCity)
                    .ThenInclude(current => current.Country)
                .Select(current => new OutboundDto
                {
                    RequestId = current.Id,
                    UserAccountId = current.UserAccountId,
                    FullName = current.UserAccount.UserProfiles.FirstOrDefault().DisplayName ?? current.UserAccount.UserProfiles.FirstOrDefault().FirstName,
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
                    MaxWidthCm = current.MaxWidthCm,
                    Status = RequestStatusEnum.FromValue(
                        current.StatusHistories
                        .Where(row => row.UserAccountId == user.Id)
                        .OrderByDescending(row => row.Id)
                        .Select(row => (int?)row.Status)
                        .FirstOrDefault() ?? (int)RequestStatusEnum.Published)
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
                .Include(current => current.UserAccount)
                    .ThenInclude(current => current.UserProfiles)
                .Include(current => current.RequestItemTypes)
                .Include(r => r.OriginCity)
                    .ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity)
                    .ThenInclude(c => c.Country)
                .Select(current => new OutboundDto
                {
                    RequestId = current.Id,
                    UserAccountId = current.UserAccountId,
                    FullName = current.UserAccount.UserProfiles.FirstOrDefault().DisplayName ?? current.UserAccount.UserProfiles.FirstOrDefault().FirstName,
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
                    MaxWidthCm = current.MaxWidthCm,
                    Status = RequestStatusEnum.FromValue(
                        current.StatusHistories
                        .Where(row => row.UserAccountId == user.Id)
                        .OrderByDescending(row => row.Id)
                        .Select(row => (int?)row.Status)
                        .FirstOrDefault() ?? (int)RequestStatusEnum.Published)
                }).ToListAsync();

            return Result<List<OutboundDto>>.Success(inboundRequests);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت پروازهای ورودی {UserId}", user.Id);
            return Result<List<OutboundDto>>.GeneralFailure("خطا در دریافت پروازهای ورودی");
        }
    }

    #region User Request Data

    public async Task<Result<RequestDetailDto>> GetRequestByIdAsync(RequestKeyDto model)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId)
                .Include(r => r.RequestSelections)
                .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                .Include(r => r.Attachments)
                .Include(r => r.AvailableOrigins).ThenInclude(o => o.City).ThenInclude(c => c.Country)
                .Include(r => r.AvailableDestinations).ThenInclude(d => d.City).ThenInclude(c => c.Country)
                .FirstOrDefaultAsync();

            if (request == null)
                return Result<RequestDetailDto>.NotFound();

            var itemTypes = (await _itemTypeRepo.Query()
                .Where(it => it.RequestId == request.Id)
                .Select(it => it.ItemType)
                .ToListAsync())
                .ToHashSet();

            var itemList = TransportableItemTypeEnum.List
                .Where(row => itemTypes.Contains(row.Value))
                .Select(it => new RequestItemTypeDto
                {
                    ItemTypeId = it.Value,
                    ItemType = it.PersianName,
                })
                .ToList();

            var userProfile = await _userProfileRepository.Query()
                .Where(p => p.UserAccountId == request.UserAccountId)
                .FirstOrDefaultAsync();

            var result = new RequestDetailDto
            {
                Id = request.Id,
                UserAccountId = request.UserAccountId,
                //CurrentStatus = RequestStatusEnum.FromValue(request.RequestSelections.OrderByDescending(order => order.Id).FirstOrDefault().Status).PersianName,
                OriginCityName = request.OriginCity?.Name,
                OriginCountryName = request.OriginCity?.Country?.Name,
                DestinationCityName = request.DestinationCity?.Name,
                DestinationCountryName = request.DestinationCity?.Country?.Name,
                ArrivalDate = request.ArrivalDate,
                DepartureDate = request.DepartureDate,
                ItemTypes = itemList,
                Attachments = request.Attachments.Select(a => new RequestAttachmentDto
                {
                    FilePath = a.FilePath,
                    FileType = a.FileType,
                    AttachmentType = a.AttachmentType.ToString()
                }).ToList(),
                AvailableOrigins = request.AvailableOrigins.Select(o => new LocationDto
                {
                    CityName = o.City?.Name,
                    CountryName = o.City?.Country?.Name
                }).ToList(),
                AvailableDestinations = request.AvailableDestinations.Select(d => new LocationDto
                {
                    CityName = d.City?.Name,
                    CountryName = d.City?.Country?.Name
                }).ToList(),
                UserDisplayName = userProfile?.DisplayName
            };

            return Result<RequestDetailDto>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت جزئیات درخواست {RequestId}", model.RequestId);
            return Result<RequestDetailDto>.GeneralFailure("جزئیات درخواست");
        }
    }

    public async Task<Result<List<UserRequestsDto>>> UserRequestsAsync(UserAccount user)
    {
        try
        {
            var result = await _requestRepository.Query()
                .Include(r => r.OriginCity)
                .Where(r => r.UserAccountId == user.Id)
                .Select(current => new UserRequestsDto
                {
                    RequestId = current.Id,
                    ArrivalDate = current.ArrivalDate,
                    DepartureDate = current.DepartureDate,
                    OriginCityName = current.OriginCity.Name,
                    DestinationCityName = current.DestinationCity.Name,
                })
                .ToListAsync();

            return Result<List<UserRequestsDto>>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های کاربر {UserId}", user.Id);
            return Result<List<UserRequestsDto>>.GeneralFailure("خطا در دریافت درخواست های کاربر");
        }
    }

    public async Task<Result<List<MyPostedSelectedDto>>> MyPostedSelectedAsync(UserAccount user)
    {
        try
        {
            var result = await _requestRepository.Query()
                .Where(r => r.UserAccountId == user.Id)
                .SelectMany(r => r.RequestSelections
                    .Where(rs => rs.UserAccountId != user.Id)
                    .GroupBy(rs => rs.UserAccountId)
                    .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault())
                    .Select(rs => new MyPostedSelectedDto
                    {
                        RequestId = r.Id,
                        RequestSelectionId = rs.Id,
                        SelectorUserAccountId = rs.UserAccountId,
                        SelectorFirstName = rs.UserAccount.UserProfiles.FirstOrDefault().FirstName,
                        SelectorLastName = rs.UserAccount.UserProfiles.FirstOrDefault().LastName,
                        LastStatus = rs.Status,
                        LastStatusStr = RequestStatusEnum.FromValue(rs.Status).PersianName
                    }))
                .ToListAsync();

            return Result<List<MyPostedSelectedDto>>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های انتخاب شده {UserId}", user.Id);
            return Result<List<MyPostedSelectedDto>>.GeneralFailure("خطا در دریافت درخواست های انتخاب شده");
        }
    }

    public async Task<Result<List<UserRequestsDto>>> MySelectionsAsync(UserAccount user)
    {
        try
        {
            var result = await _requestRepository.Query()
                .Include(r => r.OriginCity)
                .Where(r => r.RequestSelections.Any(current => current.UserAccountId == user.Id))
                .Select(current => new UserRequestsDto
                {
                    RequestId = current.Id,
                    ArrivalDate = current.ArrivalDate,
                    DepartureDate = current.DepartureDate,
                    OriginCityName = current.OriginCity.Name,
                    DestinationCityName = current.DestinationCity.Name,
                })
                .ToListAsync();

            return Result<List<UserRequestsDto>>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های کاربر {UserId}", user.Id);
            return Result<List<UserRequestsDto>>.GeneralFailure("خطا در دریافت درخواست های کاربر");
        }
    }

    #endregion User Request Data

    #region Change Status

    public async Task<Result> SelectedRequestAsync(RequestKeyDto model, UserAccount user)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            RequestSelection requestSelection = new()
            {
                UserAccount = user,
                Request = request,
                Status = RequestStatusEnum.Selected,                
            };

            await _requestSelection.AddAsync(requestSelection);
            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های کاربر {UserId}", user.Id);
            return Result.GeneralFailure("خطا در دریافت درخواست های کاربر");
        }
    }

    public async Task<Result> ConfirmedBySenderAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.ConfirmedBySender;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در تایید درخواست انتخاب شده {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در تایید درخواست انتخاب شده");
        }
    }

    public async Task<Result> RejectSelectionAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.ConfirmedBySender;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در رد کردن درخواست انتخاب شده  {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در رد درخوااست انتخاب شده");
        }
    }

    public async Task<Result> RejectByManagerAsync(RequestKeyDto model)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            var requestRecord = await _requestSelection.Query()
                .FirstOrDefaultAsync(current => current.RequestId == request.Id);

            requestRecord.Status = RequestStatusEnum.RejectedByManager;
            await _requestSelection.UpdateAsync(requestRecord);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در رد درخواست توسط مدیر  {RequestId}", model.RequestId);
            return Result.GeneralFailure("خطا در رد درخواست توسط مدیر");
        }
    }

    public async Task<Result> PublishedRequestAsync(RequestKeyDto model)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            var requestRecord = await _requestSelection.Query()
                .FirstOrDefaultAsync(current => current.RequestId == request.Id);

            requestRecord.Status = RequestStatusEnum.Published;
            await _requestSelection.UpdateAsync(requestRecord);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در انتشار درخواست {UserId}", model.RequestId);
            return Result.GeneralFailure("خطا در انتشار درخواست");
        }
    }

    public async Task<Result> ReadyToPickupAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.ReadyToPickup;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت آماده برای دریافت بار  {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت آماده برای دریافت بار");
        }
    }

    public async Task<Result> PickedUpAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.PickedUp;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت دریافت بار  {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت دریافت بار");
        }
    }

    public async Task<Result> InTransitAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.InTransit;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت در حال پرواز  {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت در حال پرواز");
        }
    }

    public async Task<Result> ReadyToDeliverAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.ReadyToDeliver;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت آماده تحویل  {RequestSelectionId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت آماده تحویل");
        }
    }

    public async Task<Result> DeliveredAsync(RequestKeyDto model, UserAccount user)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            var requestRecord = await _requestSelection.Query()
                .FirstOrDefaultAsync(current => current.UserAccountId == user.Id && current.RequestId == request.Id);

            requestRecord.Status = RequestStatusEnum.Delivered;
            await _requestSelection.UpdateAsync(requestRecord);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت تحویل شده  {RequestId}", model.RequestId);
            return Result.GeneralFailure("خطا در ثبت وضعیت تحویل شده");
        }
    }

    public async Task<Result> NotDeliveredAsync(RequestSelectionKeyDto model)
    {
        try
        {
            var request = await _requestSelection.Query()
                .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestStatusEnum.NotDelivered;
            await _requestSelection.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت عدم تحویل  {UserId}", model.RequestSelectionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت عدم تحویل");
        }
    }

    #endregion Change Status

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
            return Result<bool>.Success(false);
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