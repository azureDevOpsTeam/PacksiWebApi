using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class CurrentUserService(IRepository<UserPreferredLocation> locationRepository, IUserAccountServices userAccountServices, IRepository<UserProfile> userProfileRepository, IMiniAppServices miniAppServices, IRepository<City> cityRepository, ILogger<CurrentUserService> logger, IUserContextService userContextService) : ICurrentUserService
    {
        private readonly IMiniAppServices _miniAppServices = miniAppServices;
        private readonly IUserAccountServices _userAccountServices = userAccountServices;
        private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
        private readonly IRepository<UserPreferredLocation> _locationRepo = locationRepository;
        private readonly IRepository<City> _cityRepository = cityRepository;

        public async Task<ServiceResult> AddUserPreferredLocationAsync(PreferredLocationDto model)
        {
            try
            {
                var currentUserId = userContextService.UserId.Value;

                var cities = await _cityRepository.Query()
                    .Include(c => c.Country)
                    .Where(c => model.CityIds.Contains(c.Id))
                    .ToListAsync();

                if (cities == null || cities.Count == 0)
                    return new ServiceResult().NotFound();

                List<UserPreferredLocation> preferredLocations = new();

                foreach (var city in cities)
                {
                    preferredLocations.Add(new UserPreferredLocation
                    {
                        UserAccountId = currentUserId,
                        CityId = city.Id,
                        CountryId = city.CountryId
                    });
                }

                _locationRepo.AddRange(preferredLocations);

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Successful,
                    Message = CommonMessages.Successful
                };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(
                    logger,
                    exception,
                    CommonExceptionMessage.AddFailed("لوکیشن‌های منتخب")
                );
            }
        }

        #region MINI APP

        public async Task<Result> MiniApp_AddUserPreferredLocationAsync(CountryOfResidenceDto model)
        {
            try
            {
                var user = await _userAccountServices.GetUserAccountByTelegramIdAsync(model.TelegramId);
                if (user == null)
                    return Result.NotFound();

                var userProfile = await _userProfileRepository.Query().FirstOrDefaultAsync(current => current.UserAccountId == user.Value.Id);
                if (userProfile != null)
                {
                    userProfile.CountryOfResidenceId = model.CountryOfResidenceId;
                    await _userProfileRepository.UpdateAsync(userProfile);
                }
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "خطا در ثبت کشور مبدا {TelegramUserId}", model.TelegramId);
                return Result.GeneralFailure("خطا در ثبت کشور مبدا");
            }
        }

        public async Task<ServiceResult> MiniApp_AddUserPreferredLocationAsync(PreferredLocationDto model)
        {
            try
            {
                var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
                var user = await _userAccountServices.GetUserAccountByTelegramIdAsync(validationResult.Value.User.Id);
                if (user == null)
                    return new ServiceResult().NotFound();

                var userProfile = await _userProfileRepository.Query().FirstOrDefaultAsync(current => current.UserAccountId == user.Value.Id);
                if (model.CountryOfResidenceId.HasValue)
                    userProfile.CountryOfResidenceId = model.CountryOfResidenceId.Value;

                await _userProfileRepository.UpdateAsync(userProfile);

                if (model.CityIds.Count > 0)
                {
                    var cities = await _cityRepository.Query()
                        .Include(c => c.Country)
                        .Where(c => model.CityIds.Contains(c.Id))
                        .ToListAsync();

                    if (cities == null || cities.Count == 0)
                        return new ServiceResult().NotFound();

                    List<UserPreferredLocation> preferredLocations = new();

                    foreach (var city in cities)
                    {
                        preferredLocations.Add(new UserPreferredLocation
                        {
                            UserAccountId = user.Value.Id,
                            CityId = city.Id,
                            CountryId = city.CountryId
                        });
                    }

                    _locationRepo.AddRange(preferredLocations);
                }

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Successful,
                    Message = CommonMessages.Successful
                };
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(logger, excepotion, CommonExceptionMessage.AddFailed("لوکیشن های منتخب"));
            }
        }

        #endregion MINI APP
    }
}