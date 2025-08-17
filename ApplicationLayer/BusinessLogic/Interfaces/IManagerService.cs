namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IManagerService
    {
        Task<ServiceResult> CreateInviteCodeAsync(int? maxUsageCount = null, DateTime? expireDate = null);
    }
}