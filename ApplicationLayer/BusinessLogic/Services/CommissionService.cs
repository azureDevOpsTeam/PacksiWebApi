using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.Extensions.ServiceMessages;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class CommissionService(IRepository<Commission> commissionRepository, IRepository<UserAccount> userRepository, ILogger<CommissionService> logger) : ICommissionService
    {
        private readonly IRepository<Commission> _commissionRepository = commissionRepository;
        private readonly IRepository<UserAccount> _userRepository = userRepository;
        private readonly ILogger<CommissionService> _logger = logger;

        public async Task<ServiceResult> RegisterCommissionAsync(int requestId, int carrierUserId, decimal requestPrice)
        {
            try
            {
                var carrier = await _userRepository.Query()
                .Where(u => u.Id == carrierUserId)
                .Select(u => new { u.Id, u.InvitedByManagerId })
                .FirstOrDefaultAsync();

                if (carrier == null || carrier.InvitedByManagerId == null)
                    return new ServiceResult().Successful();

                var managerId = carrier.InvitedByManagerId.Value;

                var exists = await _commissionRepository.Query()
                    .AnyAsync(x => x.RequestId == requestId);

                if (exists)
                    return new ServiceResult().Duplicated();

                var commissionAmount = requestPrice * 0.1m; // درصد سود مدیر

                var commission = new Commission
                {
                    ManagerId = managerId,
                    CarrierUserId = carrierUserId,
                    RequestId = requestId,
                    Amount = commissionAmount
                };

                await _commissionRepository.AddAsync(commission);

                return new ServiceResult().Successful();
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("کمیسیون"));
            }
        }
    }
}