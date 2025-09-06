using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class UserAccountServices(IRepository<UserAccount> userAccountRepository, IRepository<UserProfile> userProfileRepository, IMiniAppServices miniAppServices,
        IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IUserContextService userContextService, IRepository<City> cityRepository,
        IRepository<Invitation> invitationRepository, IRepository<UserPreferredLocation> locationRepo, ILogger<UserAccountServices> logger, IMapper mapper) : IUserAccountServices
    {
        private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
        private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
        private readonly IRepository<UserPreferredLocation> _locationRepo = locationRepo;
        private readonly IRepository<Role> _roleRepository = roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository = userRoleRepository;
        private readonly IRepository<Invitation> _invitationRepository = invitationRepository;
        private readonly IRepository<City> _cityRepository = cityRepository;
        private readonly IUserContextService _userContextService = userContextService;
        private readonly IMiniAppServices _miniAppServices = miniAppServices;
        private readonly ILogger<UserAccountServices> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> RegisterWithTelegramAsync(SignUpDto model)
            => await _userAccountRepository.GetDbSet()
            .AnyAsync(row => (row.PhoneNumber == PhoneNumberHelper.ExtractPhoneParts(model.PhoneNumber)
            || row.UserName == PhoneNumberHelper.NormalizePhoneNumber(model.PhonePrefix, model.PhoneNumber))
            && row.Password == null);

        public async Task<UserAccount> GetUserAccountByIdAsync(int accountId)
            => await Task.Run(() => _userAccountRepository.GetDbSet().FirstOrDefaultAsync(row => row.Id == accountId));

        public async Task<UserAccount> GetUserAccountByPhoneNumberAsync(string phoneNumber)
            => await Task.Run(() => _userAccountRepository.GetDbSet()
            .Include(current => current.UserProfiles)
            .FirstOrDefaultAsync(row => row.PhoneNumber == PhoneNumberHelper.ExtractPhoneParts(phoneNumber)));

        public async Task<bool> ExistUserAsync(SignUpDto model)
        {
            var normalizedUserName = PhoneNumberHelper.NormalizePhoneNumber(model.PhonePrefix, model.PhoneNumber);

            var user = await _userAccountRepository.Query()
                .Where(u => u.IsDeleted == false && u.IsActive == true && u.UserName == normalizedUserName)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            var hashedPassword = HashGenerator.GenerateHashChangePassword(model.Password, user.SecurityStamp);
            return user.Password == hashedPassword;
        }

        public async Task<Result<UserAccount>> GetUserAccountByTelegramIdAsync(long telegramId)
        {
            var userAccount = await _userAccountRepository.GetDbSet()
                .Include(current => current.UserProfiles)
                .Include(current => current.UserPreferredLocations)
                .FirstOrDefaultAsync(row => row.TelegramId == telegramId);

            if (userAccount == null)
                return Result<UserAccount>.NotFound("کاربر یافت نشد");

            return Result<UserAccount>.Success(userAccount);
        }

        public async Task<UserAccount> GetUserByIdAsync(int accountId)
            => await Task.Run(() => _userAccountRepository.GetDbSet()
            .Include(curent => curent.UserProfiles)
            .Include(curent => curent.UserPreferredLocations)
            .FirstOrDefaultAsync(row => row.Id == accountId));

        public ServiceResult GetUserByValidationMethodAsync(SignInDto signInDto)
        {
            try
            {
                var result = _userAccountRepository.Query();

                if (signInDto.ValidationMethod == ValidationMethodEnum.OneTimePasswordEmail)
                    result = result.Where(current => current.Email == signInDto.UserName);
                else if (signInDto.ValidationMethod == ValidationMethodEnum.OneTimePasswordMobile)
                    result = result.Where(current => current.PhoneNumber == signInDto.UserName);
                else
                    result = result.Where(
                        current => current.UserName == PhoneNumberHelper.NormalizePhoneNumber(signInDto.PhonePrefix, signInDto.UserName));

                if (!result?.Any() ?? true)
                {
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.IncorrectUser,
                        Data = null,
                        Message = CommonMessages.IncorrectUser
                    };
                }

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Successful,
                    Data = result.FirstOrDefault(),
                    Message = CommonMessages.Successful
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Failed,
                    Data = null,
                    Message = CommonMessages.Failed
                };
            }
        }

        public async Task<ServiceResult> MergeToTelegramAccountAsync(UserAccount model, string newPassword)
        {
            try
            {
                var password = HashGenerator.GenerateSHA256HashWithSalt(newPassword, out string securityStamp);
                model.SecurityStamp = securityStamp;
                model.Password = password;
                model.SecurityCode = GenerateSecurityCode();
                model.ExpireSecurityCode = DateTime.Now.AddMinutes(10);

                await _userAccountRepository.UpdateAsync(model);
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = model, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);
                return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
            }
        }

        public async Task<ServiceResult> AddUserAccountAsync(UserAccount model)
        {
            try
            {
                model.SecurityCode = GenerateSecurityCode();
                model.ExpireSecurityCode = DateTime.Now.AddMinutes(10);
                var existUser = await _userAccountRepository.AnyAsync(current => current.UserName == model.PhoneNumber || current.PhoneNumber == model.PhoneNumber);
                if (existUser)
                    return new ServiceResult { RequestStatus = RequestStatus.Exists, Data = model, Message = CommonMessages.Exist };

                await _userAccountRepository.AddAsync(model);
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = model, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);
                return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
            }
        }

        public async Task<ServiceResult> AddProfileAsync(UserProfile model)
        {
            try
            {
                model.IsActive = true;
                await _userProfileRepository.AddAsync(model);
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = model, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);
                return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
            }
        }

        public async Task UpdateUserProfileAsync(UserProfile model)
            => await _userProfileRepository.UpdateAsync(model);

        public async Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto dto)
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var profileExists = await _userProfileRepository.Query().FirstOrDefaultAsync(x => x.UserAccountId == currentUserId);

                if (profileExists == null)
                    return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = CommonMessages.IncorrectUser };

                _mapper.Map(dto, profileExists);
                await _userProfileRepository.UpdateAsync(profileExists);

                return new ServiceResult().Successful();
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("آپدیت پروفایل"));
            }
        }

        public async Task<ServiceResult> GetValidInvitationAsync(string inviteCode, CancellationToken cancellationToken)
        {
            try
            {
                var invitation = await _invitationRepository
                .Query()
                .Where(i => i.Code == inviteCode && i.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

                if (invitation is null) return null;

                var isExpired = invitation.ExpireDate.HasValue && invitation.ExpireDate.Value < DateTime.UtcNow;
                var isUsageExceeded = invitation.MaxUsageCount.HasValue && invitation.UsageCount >= invitation.MaxUsageCount;

                return new ServiceResult().Successful((isExpired || isUsageExceeded) ? null : invitation);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("کمیسیون"));
            }
        }

        public async Task<ServiceResult> AssignRoleToUserAsync(int userId, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleRepository.Query()
                    .FirstOrDefaultAsync(r => r.RoleName == roleName, cancellationToken);

                if (role == null)
                {
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.NotFound,
                        Message = $"نقش {roleName} یافت نشد."
                    };
                }

                var userRole = new UserRole
                {
                    UserAccountId = userId,
                    RoleId = role.Id
                };

                await _userRoleRepository.AddAsync(userRole);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("تخصیص نقش"));
            }
        }

        public async Task<ServiceResult> UserInfoAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var user = await GetUserByIdAsync(currentUserId.Value);
                if (user == null)
                    return new ServiceResult().NotFound();

                var userInfo = new UserInfoDto
                {
                    UserAccountId = user.Id,
                    DisplayName = user.UserProfiles.FirstOrDefault()?.DisplayName,
                    FirstName = user.UserProfiles.FirstOrDefault()?.FirstName,
                    LastName = user.UserProfiles.FirstOrDefault()?.LastName,
                    CountryOfResidenceId = user.UserProfiles.FirstOrDefault()?.CountryOfResidenceId,
                    SetPreferredLocation = user.UserPreferredLocations.Any()
                };

                return new ServiceResult().Successful(userInfo);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("اطلاعات کاربر"));
            }
        }

        private static int GenerateSecurityCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int number = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
            return 10000 + (number % 90000);
        }

        #region Mini App

        public async Task<ServiceResult> MiniApp_AddUserAccountAsync(TelegramMiniAppUserDto model)
        {
            try
            {
                var existUser = await _userAccountRepository.AnyAsync(current => current.TelegramId == model.Id);
                if (existUser)
                    return new ServiceResult { RequestStatus = RequestStatus.Exists, Data = model, Message = CommonMessages.Exist };

                UserAccount userAccount = new()
                {
                    Avatar = model.PhotoUrl,
                    TelegramId = model.Id,
                    TelegramUserName = model.Username,
                    ConfirmEmail = false,
                    ConfirmPhoneNumber = false
                };

                await _userAccountRepository.AddAsync(userAccount);
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = userAccount, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);
                return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
            }
        }

        public async Task<ServiceResult> MiniApp_AddProfileAsync(UserProfile model)
        {
            try
            {
                model.IsActive = true;
                await _userProfileRepository.AddAsync(model);
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = model, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                _logger.LogError(message: exception.Message, CommonMessages.Failed);
                return new ServiceResult { RequestStatus = RequestStatus.Failed, Data = model, Message = CommonMessages.Failed };
            }
        }

        public async Task<ServiceResult> MiniApp_UpdateUserProfileAsync(UpdateUserProfileDto model)
        {
            try
            {
                var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
                var user = await GetUserAccountByTelegramIdAsync(validationResult.Value.User.Id);
                if (user == null)
                    return new ServiceResult().NotFound();

                var profileExists = await _userProfileRepository.Query().FirstOrDefaultAsync(x => x.UserAccountId == user.Value.Id);
                _mapper.Map(model, profileExists);
                await _userProfileRepository.UpdateAsync(profileExists);

                var requestedCityIds = model.CityIds.Distinct().ToList();

                var existingLocations = await _locationRepo.Query()
                    .Where(x => x.UserAccountId == user.Value.Id && x.CityId != null)
                    .ToListAsync();

                var existingCityIds = existingLocations.Select(x => x.CityId.Value).ToList();

                var toAddCityIds = requestedCityIds.Except(existingCityIds).ToList();

                var toRemoveCityIds = existingCityIds.Except(requestedCityIds).ToList();

                if (toAddCityIds.Count != 0)
                {
                    var cities = await _cityRepository.Query()
                        .Where(c => toAddCityIds.Contains(c.Id))
                        .ToListAsync();

                    var newLocations = cities.Select(city => new UserPreferredLocation
                    {
                        UserAccountId = user.Value.Id,
                        CityId = city.Id,
                        CountryId = city.CountryId
                    }).ToList();

                    _locationRepo.AddRange(newLocations);
                }

                if (toRemoveCityIds.Count != 0)
                {
                    var toRemoveLocations = existingLocations
                        .Where(x => toRemoveCityIds.Contains(x.CityId.Value))
                        .ToList();

                    _locationRepo.RemoveRange(toRemoveLocations);
                }

                return new ServiceResult().Successful();
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("آپدیت پروفایل"));
            }
        }

        public async Task<ServiceResult> MiniApp_UserInfoAsync()
        {
            try
            {
                var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
                var user = await GetUserAccountByTelegramIdAsync(validationResult.Value.User.Id);
                if (user == null)
                    return new ServiceResult().NotFound();

                var userInfo = new UserInfoDto
                {
                    UserAccountId = user.Value.Id,
                    DisplayName = user.Value.UserProfiles.FirstOrDefault()?.DisplayName,
                    FirstName = user.Value.UserProfiles.FirstOrDefault()?.FirstName,
                    LastName = user.Value.UserProfiles.FirstOrDefault()?.LastName,
                    ConfirmPhoneNumber = user.Value.ConfirmPhoneNumber,
                    CountryOfResidenceId = user.Value.UserProfiles.FirstOrDefault()?.CountryOfResidenceId,
                    SetPreferredLocation = user.Value.UserPreferredLocations.Count != 0
                };

                return new ServiceResult().Successful(userInfo);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("اطلاعات کاربر"));
            }
        }

        public async Task<ServiceResult> MiniApp_RequiredOperationAsync()
        {
            try
            {
                var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
                var user = await GetUserAccountByTelegramIdAsync(validationResult.Value.User.Id);
                if (user == null)
                    return new ServiceResult().NotFound();

                var userInfo = new RequiredOperationDto
                {
                    UserAccountId = user.Value.Id,
                    DisplayName = user.Value.UserProfiles.FirstOrDefault()?.DisplayName,
                    FirstName = user.Value.UserProfiles.FirstOrDefault()?.FirstName,
                    LastName = user.Value.UserProfiles.FirstOrDefault()?.LastName,
                    ConfirmPhoneNumber = user.Value.ConfirmPhoneNumber,
                    CountryOfResidenceId = user.Value.UserProfiles.FirstOrDefault()?.CountryOfResidenceId,
                    SetPreferredLocation = user.Value.UserPreferredLocations.Count != 0
                };

                return new ServiceResult().Successful(userInfo);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("اطلاعات کاربر"));
            }
        }

        public async Task<ServiceResult> MiniApp_VerifyPhoneNumberAsync(VerifyPhoneNumberDto mdoel)
        {
            try
            {
                var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();

                var user = await GetUserAccountByTelegramIdAsync(validationResult.Value.User.Id);
                if (user == null)
                    return new ServiceResult().NotFound();

                user.Value.PhoneNumber = PhoneNumberHelper.ExtractPhoneParts(mdoel.PhoneNumber);
                user.Value.PhonePrefix = PhoneNumberHelper.ExtractCountryCode(mdoel.PhoneNumber);
                user.Value.UserName = mdoel.PhoneNumber;
                user.Value.ConfirmPhoneNumber = true;

                await _userAccountRepository.UpdateAsync(user.Value);

                return new ServiceResult().Successful();
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("تایید شماره موبایل"));
            }
        }

        #endregion Mini App
    }
}