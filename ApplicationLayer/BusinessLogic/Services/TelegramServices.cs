using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class TelegramServices(IRepository<UserAccount> userAccountRepository, IRepository<TelegramUserInformation> telegramUserRepository, ILogger<UserAccountServices> logger) : ITelegramServices
    {
        private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
        private readonly IRepository<TelegramUserInformation> _telegramUserRepository = telegramUserRepository;
        private readonly ILogger<UserAccountServices> _logger = logger;

        public async Task<ServiceResult> UserAnyAsync(int telegramUserId)
        {
            var user = await _telegramUserRepository.Query()
                .Include(current => current.UserAccount)
                .Where(current => current.TelegramUserId == telegramUserId && current.UserAccount.ConfirmPhoneNumber)
                .Select(current => new TelegramInfoDto
                {
                    PhoneNumber = current.UserAccount.PhoneNumber,
                    TelegramUserId = current.TelegramUserId,
                    Language = current.Language
                }).FirstOrDefaultAsync();

            if (user != null)
                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Exists,
                    Data = user,
                    Message = CommonMessages.Successful
                };
            else
                return new ServiceResult
                {
                    RequestStatus = RequestStatus.NotFound,
                    Message = CommonMessages.NotFound
                };
        }

        public async Task<ServiceResult> VerifyTelegramAsync(TelegramUserInformation model, string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.IncorrectUser,
                        Message = CommonMessages.IncorrectUser
                    };

                var user = await _userAccountRepository.Query().FirstOrDefaultAsync(current => current.PhoneNumber == phoneNumber);

                if (user == null)
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.IncorrectUser,
                        Message = CommonMessages.IncorrectUser
                    };

                if (user.ConfirmPhoneNumber)
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.AccountConfirmed,
                        Message = CommonMessages.AccountConfirmed
                    };

                user.ConfirmPhoneNumber = true;
                user.SecurityCode = null;
                user.ExpireSecurityCode = null;

                model.UserAccountId = user.Id;

                await _telegramUserRepository.AddAsync(model);

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Successful,
                    Message = CommonMessages.Successful
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "خطا در تأیید شماره از طریق تلگرام");

                return new ServiceResult
                {
                    RequestStatus = RequestStatus.Failed,
                    Message = "خطا در پردازش اطلاعات."
                };
            }
        }
    }
}