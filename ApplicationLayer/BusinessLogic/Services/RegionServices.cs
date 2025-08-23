using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.DTOs.Regions;
using ApplicationLayer.Extensions.ServiceMessages;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class RegionServices(IRepository<Country> contryRepository, IRepository<City> cityRepository, ILogger<RegionServices> logger) : IRegionServices
    {
        private readonly IRepository<Country> _countryRepository = contryRepository;
        private readonly IRepository<City> _cityRepository = cityRepository;
        private readonly ILogger<RegionServices> _logger = logger;

        public async Task<Result<DropDownDto>> GetAllPhonePrefixAsync()
        {
            try
            {
                var getAll = await _countryRepository.Query()
                .ToListAsync();

                if (!getAll.Any())
                    return Result<DropDownDto>.NotFoundFailure();

                var dropdownItems = getAll.Select(p => new DropDownItemDto
                {
                    Text = p.PhonePrefix,
                    Value = p.Id.ToString(),
                    Icon = p.Flag
                }).ToList();

                var result = new DropDownDto
                {
                    ListItems = dropdownItems
                };

                return Result<DropDownDto>.Success(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "خطا در دریافت پیش شماره‌ها");
                return Result<DropDownDto>.GeneralFailure(CommonExceptionMessage.GetFailed("پیش شماره"));
            }
        }

        public async Task<Result<DropDownDto>> GetCountriesAsync()
        {
            try
            {
                var getAll = await _countryRepository.Query()
                .ToListAsync();

                if (!getAll.Any())
                    return Result<DropDownDto>.NotFoundFailure();

                var dropdownItems = getAll.Select(current => new DropDownItemDto
                {
                    Text = current.Name,
                    Value = current.Id.ToString(),
                    Icon = current.Flag
                }).ToList();

                var result = new DropDownDto
                {
                    ListItems = dropdownItems
                };

                return Result<DropDownDto>.Success(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "خطا در دریافت کشورها");
                return Result<DropDownDto>.GeneralFailure(CommonExceptionMessage.GetFailed("کشورها"));
            }
        }

        public async Task<Result<DropDownDto>> GetCitiesAsync(CountryKeyDto model)
        {
            try
            {
                var getAll = await _cityRepository.Query()
                    .Where(current => current.CountryId.Equals(model.Id))
                    .ToListAsync();

                if (!getAll.Any())
                    return Result<DropDownDto>.NotFoundFailure();

                var dropdownItems = getAll.Select(current => new DropDownItemDto
                {
                    Text = current.Name,
                    Value = current.Id.ToString(),
                }).ToList();

                var result = new DropDownDto
                {
                    ListItems = dropdownItems
                };

                return Result<DropDownDto>.Success(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "خطا در دریافت شهرها برای کشور {CountryId}", model.Id);
                return Result<DropDownDto>.GeneralFailure(CommonExceptionMessage.GetFailed("شهرها"));
            }
        }

        public async Task<Result<DropDownTreeDto>> GetCitiesTreeAsync()
        {
            try
            {
                var countries = await _countryRepository.Query()
                    .Include(c => c.Cities)
                    .ToListAsync();

                if (!countries.Any())
                    return Result<DropDownTreeDto>.NotFoundFailure("اطلاعات شهرها یافت نشد");

                var dropdownItems = countries.Select(country => new DropDownItemTreeDto
                {
                    Text = country.PersianName,
                    Value = country.Id.ToString(),
                    Children = country.Cities.Select(city => new DropDownItemTreeDto
                    {
                        Text = city.Name,
                        Value = city.Id.ToString()
                    }).ToList()
                }).ToList();

                var result = new DropDownTreeDto
                {
                    ListItems = dropdownItems
                };

                return Result<DropDownTreeDto>.Success(result);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "خطا در دریافت شهرها");
                return Result<DropDownTreeDto>.GeneralFailure(CommonExceptionMessage.GetFailed("شهرها"));
            }
        }
    }
}