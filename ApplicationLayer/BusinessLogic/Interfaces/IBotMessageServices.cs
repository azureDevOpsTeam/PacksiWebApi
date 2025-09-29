using ApplicationLayer.DTOs;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IBotMessageServices
{
    Task<Result<bool>> DepartureCountriesAsync(long telegramUserId);
    Task<Result<bool>> SendWelcomeMessageAsync(long telegramUserId, string referralCode = null);
}
