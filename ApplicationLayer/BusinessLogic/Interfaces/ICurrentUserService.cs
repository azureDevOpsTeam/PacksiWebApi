using ApplicationLayer.DTOs.User;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface ICurrentUserService
{
    Task<ServiceResult> AddUserPreferredLocationAsync(PreferredLocationDto model);

    Task<ServiceResult> MiniApp_AddUserPreferredLocationAsync(PreferredLocationDto model);
}