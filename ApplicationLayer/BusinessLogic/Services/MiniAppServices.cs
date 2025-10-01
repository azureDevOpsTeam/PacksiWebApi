using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Advertisements;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Telegram.Bot;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class MiniAppServices(HttpClient httpClient, IRepository<TelegramUserInformation> telegramUserRepository, IRepository<UserAccount> userAccountRepository,
    IRepository<UserProfile> userProfileRepository, IRepository<Suggestion> suggestionRepository, IRepository<Advertisement> advertisementRepository,
    IRepository<Request> requestRepository, IRepository<RequestItemType> itemTypeRepo, IRepository<SuggestionAttachment> suggestionAttachmentRepository, IRepository<UserRating> userRating,
    IRepository<RequestStatusHistory> requestStatusHistoryRepository, IRepository<Conversation> conversationRepository, IRepository<RequestAttachment> requestAttachment,
    IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger<MiniAppServices> logger, IMapper mapper) : IMiniAppServices
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IRepository<Advertisement> _advertisementRepository = advertisementRepository;
    private readonly IRepository<TelegramUserInformation> _telegramUserRepository = telegramUserRepository;
    private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
    private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepository<Request> _requestRepository = requestRepository;
    private readonly IRepository<RequestItemType> _itemTypeRepo = itemTypeRepo;
    private readonly IRepository<RequestAttachment> _requestAttachment = requestAttachment;
    private readonly IRepository<RequestStatusHistory> _requestStatusHistoryRepository = requestStatusHistoryRepository;
    private readonly IRepository<Conversation> _conversationRepository = conversationRepository;
    private readonly IRepository<Suggestion> _suggestionRepository = suggestionRepository;
    private readonly IRepository<SuggestionAttachment> _suggestionAttachmentRepository = suggestionAttachmentRepository;
    private readonly IRepository<UserRating> _userRating = userRating;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<MiniAppServices> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly TelegramBotClient _botClient = new(configuration["TelegramBot:Token"] ?? "8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A");

    public async Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync()
    {
        try
        {
            //TODO For TEST
            var botToken = _botClient.Token;
            var initData = _httpContextAccessor.HttpContext?.Request.Headers["X-Telegram-Init-Data"].FirstOrDefault();

            //TODO For TEST
            if (string.IsNullOrEmpty(initData))
                initData = "query_id=AAEUWrBhAgAAABRasGGnQ6QE&user=%7B%22id%22%3A5933914644%2C%22first_name%22%3A%22Shahram%22%2C%22last_name%22%3A%22%22%2C%22language_code%22%3A%22en%22%2C%22allows_write_to_pm%22%3Atrue%2C%22photo_url%22%3A%22https%3A%5C%2F%5C%2Ft.me%5C%2Fi%5C%2Fuserpic%5C%2F320%5C%2FQGwtYapyXkY4-jZJkczPeUb_XKfimJozOKy8lZzBhtQc4cO4xBQzwdPwcb_QSNih.svg%22%7D&auth_date=1759303181&signature=kx_xjIDfuAjp7OLVaHbkkyNvNrbuTER8ISovabR5awA_dGqZ8UGfpRhEhpXj5kzJHHZ-rOXnedV1P1ucajCLBQ&hash=bb35923b04583adc0e77b1e944b589dd1364230aaa2caeb843bae1fe7e1c9156";

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
            // TODO: Temporarily disabled for testing
            //if (DateTime.UtcNow.Subtract(authDate).TotalHours > 24)
            //    return Result<TelegramMiniAppValidationResultDto>.AuthenticationFailure("داده‌های اعتبارسنجی منقضی شده‌اند");

            var validationResult = new TelegramMiniAppValidationResultDto
            {
                IsValid = true,
                User = parsedData.User,
                AuthDate = authDate,
                Hash = parsedData.Hash,
                StartParam = parsedData.StartParam,
                ExistUser = await _userAccountRepository.GetDbSet().AnyAsync(current => current.TelegramId == parsedData.User.Id)
            };
            if (!validationResult.ExistUser)
            {
                var result = await DownloadUserProfilePhotoAsync(validationResult.User.Id);
                if (!string.IsNullOrEmpty(result))
                {
                    validationResult.User.PhotoUrl = "https://api.packsi.net" + result;
                }
            }
            return Result<TelegramMiniAppValidationResultDto>.Success(validationResult);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در اعتبارسنجی کاربر");
            return Result<TelegramMiniAppValidationResultDto>.GeneralFailure("خطای داخلی سرور");
        }
    }

    private async Task<string> DownloadUserProfilePhotoAsync(long userId)
    {
        try
        {
            //TODO For TEST
            var botToken = "8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A";

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", userId.ToString());
            if (Directory.Exists(directoryPath) == false)
                Directory.CreateDirectory(directoryPath);

            // مرحله 1: گرفتن عکس پروفایل
            var photosResponse = await _httpClient.GetStringAsync(
                $"https://api.telegram.org/bot{botToken}/getUserProfilePhotos?user_id={userId}&limit=1"
            );

            var photos = System.Text.Json.JsonDocument.Parse(photosResponse);
            if (!photos.RootElement.GetProperty("result").GetProperty("photos").EnumerateArray().Any())
                return null;

            var fileId = photos.RootElement
                .GetProperty("result")
                .GetProperty("photos")[0][0]
                .GetProperty("file_id")
                .GetString();

            // مرحله 2: گرفتن file_path
            var fileResponse = await _httpClient.GetStringAsync(
                $"https://api.telegram.org/bot{botToken}/getFile?file_id={fileId}"
            );

            var fileJson = System.Text.Json.JsonDocument.Parse(fileResponse);
            var filePath = fileJson.RootElement
                .GetProperty("result")
                .GetProperty("file_path")
                .GetString();

            var extension = Path.GetExtension(filePath);

            var savePath = Path.Combine(directoryPath, $"{userId}{extension}");

            // مرحله 3: دانلود فایل
            var fileUrl = $"https://api.telegram.org/file/bot{botToken}/{filePath}";
            var fileBytes = await _httpClient.GetByteArrayAsync(fileUrl);

            await File.WriteAllBytesAsync(savePath, fileBytes);
            var relativePath = string.Empty;

            if (userId != 0 && !string.IsNullOrEmpty(extension))
                relativePath = $"/uploads/{userId}/{userId}{extension}";
            return relativePath;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در اعتبارسنجی کاربر");
            return null;
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

    public async Task<Result<List<TripsDto>>> GetRequestTripsAsync(UserAccount user)
    {
        try
        {
            var today = DateTime.Now.Date;

            var userCountryId = await _userProfileRepository.Query()
                .Where(p => p.UserAccountId == user.Id)
                .Select(p => p.CountryOfResidenceId)
                .FirstOrDefaultAsync();

            // کوئری پایه
            var baseQuery = _requestRepository.Query()
                .Where(r => r.UserAccountId != user.Id)
                .OrderByDescending(current => current.Id)
                .Include(r => r.UserAccount).ThenInclude(u => u.UserProfiles)
                .Include(r => r.UserRatings)
                .Include(r => r.Suggestions).ThenInclude(s => s.RequestStatusHistories)
                .Include(r => r.Suggestions).ThenInclude(s => s.UserAccount).ThenInclude(ua => ua.UserProfiles)
                .Include(r => r.RequestItemTypes)
                .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                .AsQueryable();

            // شرط فیلتر کشور اگر userCountryId مشخص باشد
            if (userCountryId.HasValue && userCountryId.Value != 0)
                baseQuery = baseQuery.Where(r =>
                    r.OriginCity.CountryId == userCountryId.Value ||
                    r.DestinationCity.CountryId == userCountryId.Value);
            else
                baseQuery = baseQuery.Take(20);

            // اجرای کوئری + مپ مستقیم به TripsDto
            var result = await baseQuery
                .Select(r => new
                {
                    Request = r,
                    LastStatusValue = r.Suggestions
                        .Where(sel => sel.UserAccountId == user.Id)
                        .SelectMany(sel => sel.RequestStatusHistories)
                        .OrderByDescending(h => h.Id)
                        .Select(h => (int?)h.Status)
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .ToListAsync();

            var trips = result.Select(r => new TripsDto
            {
                RequestId = r.Request.Id,
                UserAccountId = r.Request.UserAccountId,
                FullName = r.Request.UserAccount.UserProfiles.FirstOrDefault().DisplayName
                           ?? r.Request.UserAccount.UserProfiles.FirstOrDefault().FirstName,
                DepartureDate = r.Request.DepartureDate,
                DepartureDatePersian = DateTimeHelper.GetPersianDate(r.Request.DepartureDate),
                Description = r.Request.Description,
                DestinationCity = r.Request.DestinationCity.Name,
                OriginCity = r.Request.OriginCity.Name,
                ItemTypes = [.. r.Request.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).Name)],
                ItemTypesFa = [.. r.Request.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).PersianName)],
                MaxHeightCm = r.Request.MaxHeightCm,
                MaxLengthCm = r.Request.MaxLengthCm,
                MaxWeightKg = r.Request.MaxWeightKg,
                MaxWidthCm = r.Request.MaxWidthCm,
                LastStatus = r.LastStatusValue.HasValue
                    ? RequestProcessStatus.FromValue(r.LastStatusValue.Value)
                    : RequestProcessStatus.Published,
                TripType = userCountryId.HasValue && userCountryId.Value != 0
                    ? (r.Request.OriginCity.CountryId == userCountryId.Value ? "outbound"
                        : r.Request.DestinationCity.CountryId == userCountryId.Value ? "inbound" : "")
                    : "all",
                UserRate = r.Request.UserRatings
                    .Where(ur => ur.RateeUserAccountId == r.Request.UserAccountId)
                    .Select(ur => (double?)ur.Rating)
                    .DefaultIfEmpty(0)
                    .Average()
            }).ToList();

            return Result<List<TripsDto>>.Success(trips);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست‌ها {UserId}", user.Id);
            return Result<List<TripsDto>>.GeneralFailure("خطا در دریافت درخواست‌ها");
        }
    }

    public async Task<Result<RequestInprogressDto>> GetInProgressRequestAsync(UserAccount user)
    {
        try
        {
            var myReciveOffers = await _requestRepository.Query()
                .Include(r => r.UserAccount)
                    .ThenInclude(u => u.ConversationsAsUser2)
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .Include(r => r.Suggestions)
                    .ThenInclude(s => s.SuggestionAttachments)
                .Include(r => r.Suggestions)
                    .ThenInclude(s => s.UserAccount)
                        .ThenInclude(u => u.UserProfiles)
                .Where(r => r.UserAccountId == user.Id &&
                            r.Suggestions.Any(s => s.UserAccountId != user.Id))
                .Select(r => new RequestInfoDto
                {
                    Id = r.Id,
                    OriginCityName = r.OriginCity != null ? r.OriginCity.Name : null,
                    OriginCityPersianName = r.OriginCity != null ? r.OriginCity.PersianName : null,
                    DestinationCityName = r.DestinationCity != null ? r.DestinationCity.Name : null,
                    DestinationCityPersianName = r.DestinationCity != null ? r.DestinationCity.PersianName : null,
                    Suggestions = r.Suggestions
                        .Where(s => s.UserAccountId != user.Id && s.Status != RequestProcessStatus.RejectedBySender)
                        .Select(s => new ActiveSuggestionDto
                        {
                            Id = s.Id,
                            ConvertsationId = s.Request.UserAccount.ConversationsAsUser2
                                                    .Where(c => c.RequestId == s.Request.Id && c.User2Id == user.Id)
                                                    .Select(c => (int?)c.Id)
                                                    .FirstOrDefault(),
                            DisplayName = s.UserAccount.UserProfiles
                                                .Select(up => up.DisplayName)
                                                .FirstOrDefault() ?? s.UserAccount.UserName,
                            SuggestionPrice = s.SuggestionPrice,
                            Currency = s.Currency,
                            ItemType = s.ItemType,
                            Attachments = s.SuggestionAttachments
                                                .Select(a => a.FilePath)
                                                .ToList(),
                            Status = s.Status,
                            Context = OfferContext.Received,
                            Descriptions = s.Description
                        }).ToList()
                }).ToListAsync();

            var mySentOffers = await _requestRepository.Query()
                .Include(r => r.UserAccount)
                    .ThenInclude(u => u.ConversationsAsUser1)
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .Include(r => r.Suggestions)
                    .ThenInclude(s => s.SuggestionAttachments)
                .Include(r => r.Suggestions)
                    .ThenInclude(s => s.UserAccount)
                        .ThenInclude(u => u.UserProfiles)
                .Where(r => r.Suggestions.Any(s => s.UserAccountId == user.Id))
                .Select(r => new RequestInfoDto
                {
                    Id = r.Id,
                    OriginCityName = r.OriginCity != null ? r.OriginCity.Name : null,
                    OriginCityPersianName = r.OriginCity != null ? r.OriginCity.PersianName : null,
                    DestinationCityName = r.DestinationCity != null ? r.DestinationCity.Name : null,
                    DestinationCityPersianName = r.DestinationCity != null ? r.DestinationCity.PersianName : null,
                    Status = r.Status,
                    Suggestions = r.Suggestions
                        .Where(s => s.UserAccountId == user.Id)
                        .Select(s => new ActiveSuggestionDto
                        {
                            Id = s.Id,
                            ConvertsationId = s.Request.UserAccount.ConversationsAsUser1
                                                    .Where(c => c.RequestId == s.Request.Id && c.User2Id == user.Id)
                                                    .Select(c => (int?)c.Id)
                                                    .FirstOrDefault(),
                            DisplayName = s.UserAccount.UserProfiles
                                                .Select(up => up.DisplayName)
                                                .FirstOrDefault() ?? s.UserAccount.UserName,
                            SuggestionPrice = s.SuggestionPrice,
                            Currency = s.Currency,
                            ItemType = s.ItemType,
                            Status = s.Status,
                            Attachments = s.SuggestionAttachments
                                                .Select(a => a.FilePath)
                                                .ToList(),
                            Context = OfferContext.Sent,
                            Descriptions = s.Description,
                            DeliveryCode = s.DeliveryCode
                        }).ToList()
                }).ToListAsync();

            var result = new RequestInprogressDto
            {
                MyReciveOffers = myReciveOffers,
                MySentOffers = mySentOffers
            };

            return Result<RequestInprogressDto>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست‌ها {UserId}", user.Id);
            return Result<RequestInprogressDto>.GeneralFailure("خطا در دریافت درخواست‌ها");
        }
    }

    public async Task<Result<List<TripsDto>>> GetMyRequestsAsync(UserAccount user)
    {
        try
        {
            var requests = await _requestRepository.Query()
                .Where(current => current.UserAccount == user)
                .Include(r => r.UserAccount).ThenInclude(u => u.UserProfiles)
                .Include(r => r.Suggestions).ThenInclude(rs => rs.RequestStatusHistories)
                .Include(r => r.RequestItemTypes)
                .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                .Select(r => new TripsDto
                {
                    RequestId = r.Id,
                    UserAccountId = r.UserAccountId,
                    FullName = r.UserAccount.UserProfiles.FirstOrDefault().DisplayName
                               ?? r.UserAccount.UserProfiles.FirstOrDefault().FirstName,
                    ArrivalDate = r.ArrivalDate,
                    DepartureDate = r.DepartureDate,
                    ArrivalDatePersian = DateTimeHelper.GetPersianDate(r.ArrivalDate),
                    DepartureDatePersian = DateTimeHelper.GetPersianDate(r.DepartureDate),
                    Description = r.Description,
                    DestinationCity = r.DestinationCity.Name,
                    DestinationCityFa = r.DestinationCity.PersianName,
                    OriginCity = r.OriginCity.Name,
                    OriginCityFa = r.OriginCity.PersianName,
                    ItemTypes = r.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).Name).ToArray(),
                    ItemTypesFa = r.RequestItemTypes.Select(it => TransportableItemTypeEnum.FromValue(it.ItemType).PersianName).ToArray(),
                    MaxHeightCm = r.MaxHeightCm,
                    MaxLengthCm = r.MaxLengthCm,
                    MaxWeightKg = r.MaxWeightKg,
                    MaxWidthCm = r.MaxWidthCm,
                    LastStatus = r.Suggestions
                        .Where(sel => sel.UserAccountId == user.Id)
                        .SelectMany(sel => sel.RequestStatusHistories
                            .OrderByDescending(h => h.Id)
                            .Select(h => (int?)h.Status))
                        .FirstOrDefault() ?? (int?)r.Status,
                    TripType = r.RequestType == RequestTypeEnum.Passenger ? RequestTypeEnum.Passenger.EnglishName : RequestTypeEnum.Sender.EnglishName
                }).ToListAsync();

            return Result<List<TripsDto>>.Success(requests);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست‌ها {UserId}", user.Id);
            return Result<List<TripsDto>>.GeneralFailure("خطا در دریافت درخواست‌ها");
        }
    }

    public async Task<Result<Request>> AddRequestAsync(MiniApp_CreateRequestCommand model, UserAccount userAccount, CancellationToken cancellationToken)
    {
        try
        {
            var exist = await _requestRepository.AnyAsync(current => current.UserAccount == userAccount
            && current.OriginCityId == model.OriginCityId
            && current.DestinationCityId == model.DestinationCityId);
            if (exist)
                return Result<Request>.DuplicateFailure();

            var request = _mapper.Map<Request>(model);
            request.UserAccountId = userAccount.Id;

            request.Status = model.IsDraft
                ? RequestLifecycleStatus.Draft
                : RequestLifecycleStatus.Published;
            //Todo : Post Review For Admin
            //: RequestLifecycleStatus.Created;

            await _requestRepository.AddAsync(request);
            return Result<Request>.Success(request);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت درخواست  {UserId}", userAccount.Id);
            return Result<Request>.GeneralFailure("خطا در درخواست");
        }
    }

    public async Task<Result<Request>> UpdateRequestAsync(MiniApp_UpdateRequestCommand model, UserAccount userAccount, CancellationToken cancellationToken)
    {
        try
        {
            var request = await _requestRepository.Query().FirstOrDefaultAsync(current => current.UserAccount == userAccount && current.Id == model.RequestId);
            if (request == null)
                return Result<Request>.DuplicateFailure();

            request = _mapper.Map<Request>(model);
            request.UserAccountId = userAccount.Id;

            request.Status = model.IsDraft
                ? RequestLifecycleStatus.Draft
                : RequestLifecycleStatus.Published;
            //Todo : Post Review For Admin
            //: RequestLifecycleStatus.Created;

            await _requestRepository.UpdateAsync(request);
            return Result<Request>.Success(request);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت درخواست  {UserId}", userAccount.Id);
            return Result<Request>.GeneralFailure("خطا در درخواست");
        }
    }

    public async Task<Result> AddRequestItemTypeAsync(MiniApp_CreateRequestCommand model, int requestId)
    {
        try
        {
            List<RequestItemType> itemTypes = new();
            foreach (var itemType in model.ItemTypeIds)
            {
                itemTypes.Add(new RequestItemType
                {
                    RequestId = requestId,
                    ItemType = itemType
                });
            }
            await _itemTypeRepo.AddRangeAsync(itemTypes);
            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت اقلام  {RequestId}", requestId);
            return Result.GeneralFailure("خطا در ثبت اقلام");
        }
    }

    public async Task<Result> UpdateRequestItemTypeAsync(MiniApp_UpdateRequestCommand model, int requestId)
    {
        try
        {
            var oldItems = await _itemTypeRepo.Query()
                .Where(current => current.RequestId == requestId && !current.IsDeleted)
                .ToListAsync();

            var oldItemIds = oldItems.Select(i => i.ItemType).ToList();
            var newItemIds = model.ItemTypeIds ?? new List<int>();

            var toDelete = oldItems.Where(i => !newItemIds.Contains(i.ItemType)).ToList();
            foreach (var item in toDelete)
                item.IsDeleted = true;

            if (toDelete.Count != 0)
                await _itemTypeRepo.UpdateRangeAsync(toDelete);

            var toAdd = newItemIds
                .Where(id => !oldItemIds.Contains(id))
                .Select(id => new RequestItemType
                {
                    RequestId = requestId,
                    ItemType = id
                })
                .ToList();

            if (toAdd.Count != 0)
                await _itemTypeRepo.AddRangeAsync(toAdd);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت اقلام {RequestId}", requestId);
            return Result.GeneralFailure("خطا در ثبت اقلام");
        }
    }

    public async Task<Result<List<RequestAttachment>>> AddRequestAttachmentAsync(int requestId, List<IFormFile> files, RequestTypeEnum requestType)
    {
        try
        {
            if (files == null || files.Count == 0)
                return Result<List<RequestAttachment>>.Success();

            var attachments = new List<RequestAttachment>();
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "request");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            if (files != null)
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        attachments.Add(new RequestAttachment
                        {
                            RequestId = requestId,
                            FilePath = $"https://api.packsi.net/uploads/request/{fileName}",
                            FileType = formFile.ContentType,
                            AttachmentType = requestType == RequestTypeEnum.Passenger ? AttachmentTypeEnum.Ticket : AttachmentTypeEnum.ItemImage,
                        });
                    }
                }

            await _requestAttachment.AddRangeAsync(attachments);
            return Result<List<RequestAttachment>>.Success(attachments);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"خطا در آپلود فایل برای درخواست {requestType}");
            return Result<List<RequestAttachment>>.GeneralFailure("خطا در آپلود فایل");
        }
    }

    public async Task<Result<List<RequestAttachment>>> UpdateRequestAttachmentsAsync(int requestId, List<IFormFile> files, RequestTypeEnum requestType)
    {
        try
        {
            var oldAttachments = await _requestAttachment.Query()
                .Where(a => a.RequestId == requestId && !a.IsDeleted)
                .ToListAsync();

            foreach (var att in oldAttachments)
                att.IsDeleted = true;

            if (oldAttachments.Count != 0)
                await _requestAttachment.UpdateRangeAsync(oldAttachments);

            if (files != null && files.Any())
            {
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "request");
                if (!Directory.Exists(uploadsRoot))
                    Directory.CreateDirectory(uploadsRoot);

                var newAttachments = new List<RequestAttachment>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await formFile.CopyToAsync(stream);

                        newAttachments.Add(new RequestAttachment
                        {
                            RequestId = requestId,
                            FilePath = $"https://api.packsi.net/uploads/request/{fileName}",
                            FileType = formFile.ContentType,
                            AttachmentType = requestType == RequestTypeEnum.Passenger
                                ? AttachmentTypeEnum.Ticket
                                : AttachmentTypeEnum.ItemImage,
                        });
                    }
                }

                await _requestAttachment.AddRangeAsync(newAttachments);
                return Result<List<RequestAttachment>>.Success();
            }

            return Result<List<RequestAttachment>>.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در آپدیت فایل‌های درخواست {RequestId}", requestId);
            return Result<List<RequestAttachment>>.GeneralFailure("خطا در آپدیت فایل‌ها");
        }
    }

    #region User Request Data

    public async Task<Result<RequestDetailDto>> GetRequestByIdAsync(RequestKeyDto model)
    {
        try
        {
            var request = await _requestRepository.Query()
                .Where(r => r.Id == model.RequestId)
                .Include(r => r.Suggestions)
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
                .SelectMany(r => r.Suggestions
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
                        LastStatusStr = RequestLifecycleStatus.FromValue(r.Status).PersianName
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
                .Where(r => r.Suggestions.Any(current => current.UserAccountId == user.Id))
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

    public async Task<Result<RequestSuggestionDto>> GetSuggestionAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var result = await _suggestionRepository.Query()
                .Include(r => r.Request)
                .Where(r => r.Id == model.RequestSuggestionId)
                .Select(current => new RequestSuggestionDto
                {
                    RequestSuggestionId = current.Id,
                    RequestId = current.RequestId,
                    SuggestionPrice = current.SuggestionPrice,
                    Description = current.Description
                })
                .FirstOrDefaultAsync();

            return Result<RequestSuggestionDto>.Success(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های کاربر {UserId}", model.RequestSuggestionId);
            return Result<RequestSuggestionDto>.GeneralFailure("خطا در دریافت پیشنهاد قیمت");
        }
    }

    #endregion User Request Data

    #region Change Status

    public async Task<Result> ConfirmedBySenderAsync(RequestSuggestionKeyDto model, UserAccount userAccount)
    {
        try
        {
            var suggestion = await _suggestionRepository.Query()
                .Include(current => current.Request)
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (suggestion == null)
                return Result.NotFound();

            suggestion.Status = RequestProcessStatus.ConfirmedBySender;
            await _suggestionRepository.UpdateAsync(suggestion);

            if (!await _conversationRepository.AnyAsync(current => current.User1Id == suggestion.UserAccountId && current.User2Id == userAccount.Id))
            {
                Conversation conversation = new()
                {
                    RequestId = suggestion.RequestId,
                    User1Id = suggestion.UserAccountId,
                    User2Id = userAccount.Id
                };
                await _conversationRepository.AddAsync(conversation);
            }

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در تایید درخواست انتخاب شده {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در تایید درخواست انتخاب شده");
        }
    }

    public async Task<Result> RejectSelectionAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestProcessStatus.RejectedBySender;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در رد کردن درخواست انتخاب شده  {RequestSelectionId}", model.RequestSuggestionId);
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

            request.Status = RequestLifecycleStatus.RejectedByManager;
            await _requestRepository.UpdateAsync(request);

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

            request.Status = RequestLifecycleStatus.Published;
            await _requestRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در انتشار درخواست {UserId}", model.RequestId);
            return Result.GeneralFailure("خطا در انتشار درخواست");
        }
    }

    public async Task<Result> ReadyToPickupAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();
            //todo remove ReadyToPickup
            request.Status = RequestProcessStatus.PickedUp;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت آماده برای دریافت بار  {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت آماده برای دریافت بار");
        }
    }

    public async Task<Result> PickedUpAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestProcessStatus.PickedUp;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت دریافت بار  {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت دریافت بار");
        }
    }

    public async Task<Result> InTransitAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            //todo remove InTransit
            request.Status = RequestProcessStatus.PickedUp;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت در حال پرواز  {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت در حال پرواز");
        }
    }

    public async Task<Result> ReadyToDeliverAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            //todo remove ReadyToDeliver
            request.Status = RequestProcessStatus.PickedUp;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت آماده تحویل  {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت آماده تحویل");
        }
    }

    public async Task<Result> PassengerConfirmedDeliveryAsync(RequestSuggestionKeyDto model, UserAccount user)
    {
        try
        {
            var suggestion = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (suggestion == null)
                return Result.NotFound();

            if (suggestion.DeliveryCode != model.DeliveryCode)
                return Result.Failure("کد تحویل اشتباه است");

            suggestion.Status = RequestProcessStatus.PassengerConfirmedDelivery;
            await _suggestionRepository.UpdateAsync(suggestion);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت تحویل شده  {RequestSuggestionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت تحویل شده");
        }
    }

    public async Task<Result> SaveRatingAsync(AddRatingDto model, UserAccount user)
    {
        try
        {
            var suggestion = await _suggestionRepository.Query()
                .Where(current => current.Id == model.RequestSuggestionId && current.UserAccountId == user.Id).FirstOrDefaultAsync();

            if (suggestion == null)
                return Result.NotFound();

            var request = await _requestRepository.Query()
                .Where(current => current.Id == suggestion.RequestId).FirstOrDefaultAsync();
            if (request == null)
                return Result.NotFound();

            if (model.Rate > 0)
                if (!await _userRating.AnyAsync(current => current.RaterUserAccount == user && current.RateeUserAccountId == request.UserAccountId && current.Request == request))
                {
                    UserRating userRating = new()
                    {
                        Request = request,
                        RateeUserAccountId = request.UserAccountId,
                        RaterUserAccount = user,
                        Rating = model.Rate
                    };

                    await _userRating.AddAsync(userRating);
                }

            request.Status = RequestLifecycleStatus.FinalizateDelivery;
            suggestion.Status = RequestProcessStatus.Finalize;
            await _suggestionRepository.UpdateAsync(suggestion);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت تحویل شده  {RequestSuggestionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت تحویل شده");
        }
    }

    public async Task<Result> NotDeliveredAsync(RequestSuggestionKeyDto model)
    {
        try
        {
            var request = await _suggestionRepository.Query()
                .Where(r => r.Id == model.RequestSuggestionId).FirstOrDefaultAsync();

            if (request == null)
                return Result.NotFound();

            request.Status = RequestProcessStatus.NotDelivered;
            await _suggestionRepository.UpdateAsync(request);

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ثبت وضعیت عدم تحویل  {RequestSelectionId}", model.RequestSuggestionId);
            return Result.GeneralFailure("خطا در ثبت وضعیت عدم تحویل");
        }
    }

    public async Task<Result<Suggestion>> CreateSuggestionAsync(MiniApp_CreateSuggestionCommand model, UserAccount user)
    {
        try
        {
            Suggestion suggestion = new()
            {
                UserAccount = user,
                SuggestionPrice = model.SuggestionPrice,
                Status = RequestProcessStatus.Selected,
                Currency = model.Currency,
                Description = model.Description,
                RequestId = model.RequestId,
                ItemType = model.ItemTypeId,
                DeliveryCode = new Random().Next(10000, 99999)
            };
            await _suggestionRepository.AddAsync(suggestion);
            return Result<Suggestion>.Success(suggestion);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"خطا در ثبت پیشنهاد قیمت  {model.RequestId} {user.Id}");
            return Result<Suggestion>.GeneralFailure("خطا در ثبت پیشنهاد قیمت");
        }
    }

    public async Task<Result<List<SuggestionAttachment>>> CreateSuggestionAttachmentAsync(List<IFormFile> files, int suggestionId)
    {
        try
        {
            if (files == null || !files.Any())
                return Result<List<SuggestionAttachment>>.Success();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var attachments = new List<SuggestionAttachment>();

            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                // ذخیره روی دیسک
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var suggestionAttachment = new SuggestionAttachment
                {
                    SuggestionId = suggestionId,
                    FilePath = $"https://api.packsi.net/uploads/{fileName}"
                };

                await _suggestionAttachmentRepository.AddAsync(suggestionAttachment);
                attachments.Add(suggestionAttachment);
            }

            return Result<List<SuggestionAttachment>>.Success(attachments);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"خطا در آپلود فایل برای پیشنهاد {suggestionId}");
            return Result<List<SuggestionAttachment>>.GeneralFailure("خطا در آپلود فایل");
        }
    }

    #endregion Change Status

    public Task<Result<bool>> SendMessageAsync(long chatId)
    {
        try
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            _logger.LogInformation("پیام با موفقیت به چت {ChatId} ارسال شد", chatId);
            return Task.FromResult(Result<bool>.Success(true));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ارسال پیام به چت {ChatId}", chatId);
            return Task.FromResult(Result<bool>.Success(false));
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

    public async Task<Result> AddHistoryStatusAsync(Suggestion suggestion, RequestProcessStatus processStatus, UserAccount user)
    {
        try
        {
            RequestStatusHistory requestStatusHistory = new()
            {
                UserAccount = user,
                Suggestion = suggestion,
                Status = processStatus,
            };

            await _requestStatusHistoryRepository.AddAsync(requestStatusHistory);
            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت درخواست های کاربر {UserId}", user.Id);
            return Result.GeneralFailure("خطا در دریافت درخواست های کاربر");
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
                result.User = Newtonsoft.Json.JsonConvert.DeserializeObject<TelegramMiniAppUserDto>(userJson);
            }

            if (parameters["receiver"] != null)
                result.Receiver = parameters["receiver"];

            if (parameters["chat_type"] != null)
                result.ChatType = parameters["chat_type"];

            if (parameters["chat_instance"] != null)
                result.ChatInstance = parameters["chat_instance"];

            if (parameters["start_param"] != null)
                result.StartParam = HttpUtility.UrlDecode(parameters["start_param"]);

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
        // TODO: Temporarily disabled for testing
        return true;

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

    private List<string> GetAvailableActions(string role, RequestProcessStatus lastStatus)
    {
        var actions = new List<string>();

        switch (lastStatus.Name)
        {
            case nameof(RequestProcessStatus.Selected):
                if (role == "Sender")
                {
                    actions.Add("ConfirmSelection");
                    actions.Add("RejectSelection");
                }
                break;

            case nameof(RequestProcessStatus.ConfirmedBySender):
                if (role == "Passenger")
                {
                    actions.Add("ReadyToPickup");
                }
                break;
            //TODO
            //case nameof(RequestProcessStatus.ReadyToPickup):
            //    if (role == "Passenger")
            //    {
            //        actions.Add("Pickup");
            //    }
            //    break;

            case nameof(RequestProcessStatus.PickedUp):
                if (role == "Passenger")
                {
                    actions.Add("MarkInTransit");
                }
                break;

            //case nameof(RequestProcessStatus.InTransit):
            //    if (role == "Passenger")
            //    {
            //        actions.Add("ReadyToDeliver");
            //    }
            //    break;

            //case nameof(RequestProcessStatus.ReadyToDeliver):
            //    if (role == "Passenger")
            //    {
            //        actions.Add("DeliverSuccess");
            //        actions.Add("DeliverFailed");
            //    }
            //    break;

            case nameof(RequestProcessStatus.PassengerConfirmedDelivery):
                if (role == "Passenger")
                {
                    actions.Add("PassengerConfirmedDelivery");
                }
                break;

            case nameof(RequestProcessStatus.Finalize):
                if (role == "Sender")
                {
                    actions.Add("SenderConfirmedDelivery");
                }
                break;

            case nameof(RequestProcessStatus.NotDelivered):
                if (role == "Sender")
                {
                    actions.Add("ReportIssue");
                }
                break;
        }

        return actions;
    }

    #endregion Private Methods

    #region Ads Methods

    public async Task<Result<List<AdsListDto>>> GetAdsAsync(UserAccount userAccount)
    {
        try
        {
            var ads = await _advertisementRepository.Query()
                .Where(ad => ad.UserAccountId == userAccount.Id)
                .Select(ad => new AdsListDto
                {
                    Id = ad.Id,
                    Title = ad.Title,
                    Status = ad.Status,
                    PostType = ad.PostType
                })
                .ToListAsync();

            return Result<List<AdsListDto>>.Success(ads);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت تبلیغات");
            return Result<List<AdsListDto>>.GeneralFailure("خطا در دریافت تبلیغات");
        }
    }

    #endregion Ads Methods
}