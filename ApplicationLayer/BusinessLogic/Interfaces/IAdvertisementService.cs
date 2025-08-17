using ApplicationLayer.DTOs.Advertisements;
using ApplicationLayer.DTOs.BaseDTOs;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IAdvertisementService
{
    Task<ServiceResult> AcceptAsync(KeyDto model);

    Task<ServiceResult> CreateAsync(CreateAdvertisementDto model);

    Task<ServiceResult> DeleteAsync(KeyDto model);

    Task<ServiceResult> GetAllAsync(GetAllFilterDto model);

    Task<ServiceResult> GetByIdAsync(KeyDto model);

    Task<ServiceResult> PublishAsync(KeyDto model);

    Task<ServiceResult> RejectAsync(KeyDto model);
}