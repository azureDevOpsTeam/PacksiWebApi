using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IBotMessageServices
{
    Task<Result<bool>> DepartureCountriesAsync(long telegramUserId);

    Task<Result<bool>> SendWelcomeMessageAsync(RegisterReferralDto Model);
}
