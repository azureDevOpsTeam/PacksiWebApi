using ApplicationLayer.DTOs.Identity;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IIdentityService
    {
        ServiceResult AuthenticateOneTimePassword(SignInDto signInViewModel, UserAccount userAccount);

        Task<ServiceResult> AuthenticateUserInformationAsync(SignInDto signInViewModel, UserAccount userAccount);

        (string jwtToken, string tokenId) TokenGenerator(string userName, int userId, List<string> roles);

        Task<(string jwtToken, string tokenId)> TokenRequestGeneratorAsync(string userName, int userAccountId);
    }
}