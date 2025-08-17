namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface ICommissionService
{
    Task<ServiceResult> RegisterCommissionAsync(int requestId, int carrierUserId, decimal requestPrice);
}