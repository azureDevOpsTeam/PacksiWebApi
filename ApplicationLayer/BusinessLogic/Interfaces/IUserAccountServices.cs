using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.DTOs.User;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IUserAccountServices
    {
        Task<UserAccount> GetUserAccountByIdAsync(int accountId);

        ServiceResult GetUserByValidationMethodAsync(SignInDto signInViewModel);

        Task<ServiceResult> AddProfileAsync(UserProfile model);

        Task<ServiceResult> AddUserAccountAsync(UserAccount model);

        Task<ServiceResult> GetValidInvitationAsync(string inviteCode, CancellationToken cancellationToken);

        Task<ServiceResult> AssignRoleToUserAsync(int userId, string roleName, CancellationToken cancellationToken);

        Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto dto);

        Task<ServiceResult> UserInfoAsync();

        Task<UserAccount> GetUserAccountByTelegramIdAsync(long telegramId);

        #region Mini App

        Task<ServiceResult> MiniApp_AddUserAccountAsync(TelegramMiniAppUserDto model);

        Task<ServiceResult> MiniApp_AddProfileAsync(UserProfile model);

        Task<ServiceResult> MiniApp_UserInfoAsync();

        #endregion Mini App
    }
}