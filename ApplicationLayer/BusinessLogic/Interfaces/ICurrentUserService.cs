using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.User;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface ICurrentUserService
{
    Task<ServiceResult> AddUserPreferredLocationAsync(PreferredLocationDto model);
    Task<Result> MiniApp_AddDepartureLocationAsync(CountryOfResidenceDto model);
    Task<ServiceResult> MiniApp_AddUserPreferredLocationAsync(PreferredLocationDto model);

    Task<Result> MiniApp_AddUserPreferredLocationAsync(CountryOfResidenceDto model);
}