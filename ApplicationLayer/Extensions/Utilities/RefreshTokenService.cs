using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApplicationLayer.Extensions.Utilities
{
    [InjectAsScoped]
    public class RefreshTokenService(IConfiguration iConfiguration,
                                     TokenValidationParameters tokenValidationParameters,
                                     ILogger<RefreshTokenService> logger,
                                     IRepository<RefreshToken> _refreshTokenRepository) : IRefreshTokenService
    {
        private readonly IConfiguration _iConfiguration = iConfiguration;
        private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters;
        private readonly ILogger<RefreshTokenService> _logger = logger;
        private readonly IRepository<RefreshToken> _refreshTokenRepository = _refreshTokenRepository;

        public Result<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                _refreshTokenRepository.Add(refreshToken);
                return Result<RefreshToken>.Success(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در افزودن RefreshToken");
                return Result<RefreshToken>.GeneralFailure("خطا در افزودن RefreshToken");
            }
        }

        public Result<RefreshToken> UpdateRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                _refreshTokenRepository.Update(refreshToken);
                return Result<RefreshToken>.Success(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در بروزرسانی RefreshToken");
                return Result<RefreshToken>.GeneralFailure("خطا در بروزرسانی RefreshToken");
            }
        }

        public RefreshToken RefreshTokenGenerator(int userId, string tokenId)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";
            var randomString = new string(Enumerable.Repeat(chars, 23).Select(s => s[random.Next(s.Length)]).ToArray());

            var refreshToken = new RefreshToken()
            {
                JwtId = tokenId,
                Token = randomString,
                ExpiryDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_iConfiguration.GetSection("JWT:RefreshTokenExpirationTimeInMinutes").Value)),
                IsRevoked = false,
                IsUsed = false,
                UserAccountId = userId,
            };
            return refreshToken;
        }

        public async Task<Result<RefreshToken>> VerifyToken(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Create a copy of token validation parameters for refresh token validation
                var refreshTokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = _tokenValidationParameters.ValidateIssuer,
                    ValidateAudience = _tokenValidationParameters.ValidateAudience,
                    ValidateIssuerSigningKey = _tokenValidationParameters.ValidateIssuerSigningKey,
                    ValidIssuer = _tokenValidationParameters.ValidIssuer,
                    ValidAudience = _tokenValidationParameters.ValidAudience,
                    IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,
                    ValidateLifetime = false, // Only for refresh token validation
                    ClockSkew = _tokenValidationParameters.ClockSkew,
                    RequireExpirationTime = _tokenValidationParameters.RequireExpirationTime,
                    RequireSignedTokens = _tokenValidationParameters.RequireSignedTokens
                };

                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.AccessTokens, refreshTokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                    {
                        return Result<RefreshToken>.GeneralFailure("الگوریتم امضای توکن نامعتبر است");
                    }
                }

                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = TimeHelper.UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return Result<RefreshToken>.Failure(new Error("REFRESH_NOT_REQUIRED", "نیازی به تازه‌سازی توکن نیست", RequestStatus.RefreshNotRequired));
                }

                var storedToken = await _refreshTokenRepository
                    .GetDbSet()
                    .FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken is null)
                {
                    return Result<RefreshToken>.Failure(new Error("INVALID_TOKEN", "توکن نامعتبر است", RequestStatus.InvalidToken));
                }

                if (storedToken.IsUsed)
                {
                    return Result<RefreshToken>.Failure(new Error("INVALID_TOKEN", "توکن قبلاً استفاده شده است", RequestStatus.InvalidToken));
                }

                if (storedToken.IsRevoked)
                {
                    return Result<RefreshToken>.Failure(new Error("INVALID_TOKEN", "توکن لغو شده است", RequestStatus.InvalidToken));
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (jti is null || storedToken.JwtId != jti)
                {
                    return Result<RefreshToken>.Failure(new Error("INVALID_TOKEN", "شناسه توکن نامعتبر است", RequestStatus.InvalidToken));
                }

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return Result<RefreshToken>.ExpiredTokenFailure("توکن منقضی شده است");
                }

                storedToken.IsUsed = true;
                var updateResult = UpdateRefreshToken(storedToken);

                if (updateResult.IsFailure)
                {
                    return Result<RefreshToken>.GeneralFailure("خطا در بروزرسانی وضعیت توکن");
                }

                return Result<RefreshToken>.Success(storedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در تأیید توکن");
                return Result<RefreshToken>.GeneralFailure("خطا در تأیید توکن");
            }
        }

        public Result RemoveExpiredTokensFromDatabase()
        {
            try
            {
                var expiredTokens = _refreshTokenRepository.GetDbSet()
                    .Where(t => t.ExpiryDate < DateTime.UtcNow || t.IsUsed);

                _refreshTokenRepository.DeleteRangeFromDatabase(expiredTokens);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف توکن‌های منقضی");
                return Result.GeneralFailure("خطا در حذف توکن‌های منقضی");
            }
        }

        public async Task<Result> RevokeRefreshTokenByUserId(int userId)
        {
            try
            {
                var refreshTokens = await _refreshTokenRepository
                    .GetDbSet()
                    .Where(r => r.UserAccountId == userId)
                    .ToListAsync();

                if (!refreshTokens?.Any() ?? true)
                {
                    return Result.NotFoundFailure("هیچ توکنی برای این کاربر یافت نشد");
                }

                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                    var updateResult = UpdateRefreshToken(token);
                    if (updateResult.IsFailure)
                    {
                        return Result.GeneralFailure("خطا در لغو توکن‌ها");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در لغو توکن‌های کاربر");
                return Result.GeneralFailure("خطا در لغو توکن‌های کاربر");
            }
        }
    }
}