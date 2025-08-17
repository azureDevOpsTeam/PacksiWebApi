using ApplicationLayer.DTOs;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IRefreshTokenService
{
    Result<RefreshToken> AddRefreshToken(RefreshToken refreshToken);

    RefreshToken RefreshTokenGenerator(int userId, string tokenId);

    Result RemoveExpiredTokensFromDatabase();

    Task<Result> RevokeRefreshTokenByUserIdAsync(int userId);

    Result UpdateRefreshToken(RefreshToken refreshToken);

    Task<Result<RefreshToken>> VerifyTokenAsync(TokenRequestDto tokenRequest);

    ServiceResult AddRefreshTokenLegacy(RefreshToken refreshToken);
}