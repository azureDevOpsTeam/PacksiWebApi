using ApplicationLayer.DTOs.User;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface ICurrentUserService
{
    Task<ServiceResult> AddUserPreferredLocation(AddUserPreferredLocationDto model);
}