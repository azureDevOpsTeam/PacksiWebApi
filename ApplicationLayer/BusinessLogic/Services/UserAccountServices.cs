using ApplicationLayer.BusinessLogic.Interfaces;
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
using System;
using System.Security.Cryptography;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class UserAccountServices(IRepository<UserAccount> userAccountRepository, IRepository<UserProfile> userProfileRepository, IMiniAppServices miniAppServices,
        IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IUserContextService userContextService,
        IRepository<Invitation> invitationRepository, ILogger<UserAccountServices> logger, IMapper mapper) : IUserAccountServices
    {
        private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
        private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
        private readonly IRepository<Role> _roleRepository = roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository = userRoleRepository;
        private readonly IRepository<Invitation> _invitationRepository = invitationRepository;
        private readonly IUserContextService _userContextService = userContextService;
        private readonly IMiniAppServices _miniAppServices = miniAppServices;
        private readonly ILogger<UserAccountServices> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task<UserAccount> GetUserAccountByIdAsync(int accountId)
            => await Task.Run(() => _userAccountRepository.GetDbSet().FirstOrDefaultAsync(row => row.Id == accountId));

        public async Task<UserAccount> GetUserAccountByTelegramIdAsync(long telegramId)
            => await Task.Run(() => _userAccountRepository.GetDbSet().FirstOrDefaultAsync(row => row.TelegramId == telegramId));

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

        public async Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto dto)
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var profileExists = await _userProfileRepository.Query().FirstOrDefaultAsync(x => x.UserAccountId == currentUserId);

                if (profileExists == null)
                    return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = CommonMessages.IncorrectUser };

                profileExists = _mapper.Map<UserProfile>(dto);
                _userProfileRepository.Update(profileExists);

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
                    CountryOfResidenceId = user.UserProfiles.FirstOrDefault()?.CountryOfResidenceId.Value,
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
                    UserAccountId = user.Id,
                    DisplayName = user.UserProfiles.FirstOrDefault()?.DisplayName,
                    FirstName = user.UserProfiles.FirstOrDefault()?.FirstName,
                    LastName = user.UserProfiles.FirstOrDefault()?.LastName,
                    ConfirmPhoneNumber = user.ConfirmPhoneNumber,
                    CountryOfResidenceId = user.UserProfiles.FirstOrDefault()?.CountryOfResidenceId.Value,
                    SetPreferredLocation = user.UserPreferredLocations.Count != 0
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

                user.PhoneNumber = PhoneNumberHelper.RemoveLeadingZero(mdoel.PhoneNumber);
                user.UserName = mdoel.PhoneNumber;
                user.ConfirmPhoneNumber = true;

                _userAccountRepository.Update(user);

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