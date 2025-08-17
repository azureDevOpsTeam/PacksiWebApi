using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.User;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class CurrentUserService(IRepository<UserPreferredLocation> locationRepository, ILogger<RefreshTokenService> logger, IUserContextService userContextService) : ICurrentUserService
    {
        private readonly IRepository<UserPreferredLocation> _locationRepo = locationRepository;
        private readonly ILogger<RefreshTokenService> _logger = logger;

        public async Task<ServiceResult> AddUserPreferredLocation(AddUserPreferredLocationDto model)
        {
            try
            {
                var currentUserId = userContextService.UserId.Value;
                var entity = new UserPreferredLocation
                {
                    UserAccountId = currentUserId,
                    CountryId = model.CountryId,
                    CityId = model.CityId
                };

                await _locationRepo.AddAsync(entity);
                return new ServiceResult()
                {
                    RequestStatus = RequestStatus.Successful,
                    Message = CommonMessages.Successful,
                };
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("لوکیشن های منتخب"));
            }
        }
    }
}