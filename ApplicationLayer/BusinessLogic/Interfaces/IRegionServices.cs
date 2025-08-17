using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.DTOs.Regions;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IRegionServices
    {
        Task<Result<DropDownDto>> GetAllPhonePrefixAsync();

        Task<Result<DropDownDto>> GetCitiesAsync(CountryKeyDto model);

        Task<Result<DropDownTreeDto>> GetCitiesTreeAsync();

        Task<Result<DropDownDto>> GetCountriesAsync();
    }
}