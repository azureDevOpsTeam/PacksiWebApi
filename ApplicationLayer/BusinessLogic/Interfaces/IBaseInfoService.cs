
namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IBaseInfoService
{
    Task<ServiceResult> TransportableItemAsync();
}