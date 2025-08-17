using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class BaseInfoService(ILogger<BaseInfoService> logger) : IBaseInfoService
{
    private readonly ILogger<BaseInfoService> _logger = logger;

    public async Task<ServiceResult> TransportableItemAsync()
    {
        try
        {
            var getAll = TransportableItemTypeEnum.List;

            if (!getAll.Any())
                return await Task.FromResult(new ServiceResult().NotFound(getAll));

            var dropdownItems = getAll.Select(current => new DropDownItemDto
            {
                Text = current.PersianName,
                Value = current.Value.ToString(),
            }).ToList();

            var result = new DropDownDto
            {
                ListItems = dropdownItems
            };

            return await Task.FromResult(new ServiceResult().Successful(result));
        }
        catch (Exception exception)
        {
            return await Task.FromResult(new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.GetFailed("کشورها")));
        }
    }
}