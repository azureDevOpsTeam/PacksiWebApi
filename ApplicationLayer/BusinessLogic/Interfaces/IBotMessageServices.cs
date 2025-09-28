using ApplicationLayer.DTOs;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IBotMessageServices
{
    Task<Result<bool>> SendWelcomeMessageAsync(long telegramUserId, string referralCode = null);
}
