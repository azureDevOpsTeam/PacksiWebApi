using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface ITelegramServices
    {
        Task<ServiceResult> UserAnyAsync(int telegramUserId);

        Task<ServiceResult> VerifyTelegramAsync(TelegramUserInformation model, string phoneNumber);
    }
}