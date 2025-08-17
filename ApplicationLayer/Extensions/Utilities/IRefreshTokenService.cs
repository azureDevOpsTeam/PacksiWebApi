using ApplicationLayer.DTOs;
using DomainLayer.Entities;

namespace ApplicationLayer.Extensions.Utilities
{
    public interface IRefreshTokenService
    {
        Result<RefreshToken> AddRefreshToken(RefreshToken refreshToken);

        RefreshToken RefreshTokenGenerator(int userId, string tokenId);

        Result RemoveExpiredTokensFromDatabase();

        Task<Result> RevokeRefreshTokenByUserId(int userId);

        Result<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken);

        Task<Result<RefreshToken>> VerifyToken(TokenRequestDto tokenRequest);
    }
}