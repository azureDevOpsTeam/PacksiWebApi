using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.DTOs.User;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IUserAccountServices
    {
        Task<bool> RegisterWithTelegramAsync(SignUpDto model);

        Task<UserAccount> GetUserAccountByIdAsync(int accountId);

        ServiceResult GetUserByValidationMethodAsync(SignInDto signInViewModel);

        Task<ServiceResult> AddProfileAsync(UserProfile model);

        Task<ServiceResult> AddUserAccountAsync(UserAccount model);

        Task<ServiceResult> GetValidInvitationAsync(string inviteCode, CancellationToken cancellationToken);

        Task<ServiceResult> AssignRoleToUserAsync(int userId, string roleName, CancellationToken cancellationToken);

        Task<ServiceResult> UpdateUserProfileAsync(UpdateUserProfileDto dto);

        Task<ServiceResult> UserInfoAsync();

        Task<Result<UserAccount>> GetUserAccountByTelegramIdAsync(long telegramId);

        Task<ServiceResult> MergeToTelegramAccountAsync(UserAccount model, string newPassword);

        #region Mini App

        Task<ServiceResult> MiniApp_AddUserAccountAsync(TelegramMiniAppUserDto model);

        Task<ServiceResult> MiniApp_AddProfileAsync(UserProfile model);

        Task<ServiceResult> MiniApp_UserInfoAsync();

        Task<ServiceResult> MiniApp_VerifyPhoneNumberAsync(VerifyPhoneNumberDto mdoel);

        Task<ServiceResult> MiniApp_RequiredOperationAsync();

        Task<ServiceResult> MiniApp_UpdateUserProfileAsync(UpdateUserProfileDto model);

        Task<UserAccount> GetUserAccountByPhoneNumberAsync(string phoneNumber);

        Task UpdateUserProfileAsync(UserProfile model);

        Task<bool> ExistUserAsync(SignUpDto model);

        Task<Result<UserAccount>> GetUserAccountInviterAsync(string invideCode);

        Task<Result<Referral>> GetReferralAsync(long telegramId);

        Task<Result> GetExistReferralAsync(long telegramId);

        Task<Result> AddReferralUserAsync(long InviterTelegramId, RegisterReferralDto model);
        Task<Result<int>> GetReferralCountAsync(long telegramId);

        #endregion Mini App
    }
}