using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class ManagerService(IRepository<Invitation> invitationRepository, ILogger<ManagerService> logger, IUserContextService userContextService) : IManagerService
    {
        private readonly IRepository<Invitation> invitationRepository = invitationRepository;
        private readonly ILogger<ManagerService> _logger = logger;
        private readonly IUserContextService _userContextService = userContextService;

        public async Task<ServiceResult> CreateInviteCodeAsync(int? maxUsageCount = null, DateTime? expireDate = null)
        {
            try
            {
                var inviterId = _userContextService.UserId;

                if (!inviterId.HasValue)
                    return new ServiceResult { RequestStatus = RequestStatus.IncorrectUser, Message = CommonMessages.IncorrectUser };

                var invite = new Invitation
                {
                    Code = Guid.NewGuid().ToString("N")[..15],
                    InviterUserId = inviterId.Value,
                    MaxUsageCount = maxUsageCount,
                    ExpireDate = expireDate,
                    IsActive = true
                };

                await invitationRepository.AddAsync(invite);

                return new ServiceResult().Successful(invite.Code);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("کمیسیون"));
            }
        }
    }
}