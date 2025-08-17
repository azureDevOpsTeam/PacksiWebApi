using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApplicationLayer.BusinessLogic.Services
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

        public Result UpdateRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                _refreshTokenRepository.Update(refreshToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در به‌روزرسانی RefreshToken");
                return Result.GeneralFailure("خطا در به‌روزرسانی RefreshToken");
            }
        }

        public RefreshToken RefreshTokenGenerator(int userId, string tokenId)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";
            var randomString = new string(Enumerable.Repeat(chars, 23)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return new RefreshToken()
            {
                JwtId = tokenId,
                Token = randomString,
                ExpiryDate = DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_iConfiguration.GetSection("JWT:RefreshTokenExpirationTimeInMinutes").Value)),
                IsRevoked = false,
                IsUsed = false,
                UserAccountId = userId,
            };
        }

        public async Task<Result<RefreshToken>> VerifyTokenAsync(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // ایجاد پارامترهای اعتبارسنجی برای RefreshToken
                var refreshTokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = _tokenValidationParameters.ValidateIssuer,
                    ValidateAudience = _tokenValidationParameters.ValidateAudience,
                    ValidateIssuerSigningKey = _tokenValidationParameters.ValidateIssuerSigningKey,
                    ValidIssuer = _tokenValidationParameters.ValidIssuer,
                    ValidAudience = _tokenValidationParameters.ValidAudience,
                    IssuerSigningKey = _tokenValidationParameters.IssuerSigningKey,
                    ValidateLifetime = false, // فقط برای اعتبارسنجی RefreshToken
                    ClockSkew = _tokenValidationParameters.ClockSkew,
                    RequireExpirationTime = _tokenValidationParameters.RequireExpirationTime,
                    RequireSignedTokens = _tokenValidationParameters.RequireSignedTokens
                };

                var tokenInVerification = jwtTokenHandler.ValidateToken(
                    tokenRequest.AccessTokens,
                    refreshTokenValidationParameters,
                    out var validatedToken);

                // بررسی الگوریتم امضا
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                    {
                        return Result<RefreshToken>.GeneralFailure("الگوریتم امضای نامعتبر");
                    }
                }

                // بررسی تاریخ انقضا
                var utcExpiryDate = long.Parse(tokenInVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value ?? "0");
                var expiryDate = TimeHelper.UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return Result<RefreshToken>.GeneralFailure("Token هنوز منقضی نشده است");
                }

                // جستجوی RefreshToken در دیتابیس
                var storedToken = await _refreshTokenRepository
                    .GetDbSet()
                    .FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken is null)
                {
                    return Result<RefreshToken>.NotFoundFailure("RefreshToken یافت نشد");
                }

                // بررسی وضعیت Token
                if (storedToken.IsUsed)
                {
                    return Result<RefreshToken>.ValidationFailure("RefreshToken قبلاً استفاده شده است");
                }

                if (storedToken.IsRevoked)
                {
                    return Result<RefreshToken>.ValidationFailure("RefreshToken لغو شده است");
                }

                // بررسی JTI
                var jti = tokenInVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(jti) || storedToken.JwtId != jti)
                {
                    return Result<RefreshToken>.ValidationFailure("JTI نامعتبر است");
                }

                // بررسی تاریخ انقضای RefreshToken
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return Result<RefreshToken>.ExpiredTokenFailure("RefreshToken منقضی شده است");
                }

                // علامت‌گذاری Token به عنوان استفاده شده
                storedToken.IsUsed = true;
                var updateResult = UpdateRefreshToken(storedToken);

                if (updateResult.IsFailure)
                {
                    return Result<RefreshToken>.GeneralFailure("خطا در به‌روزرسانی RefreshToken");
                }

                return Result<RefreshToken>.Success(storedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در اعتبارسنجی Token");
                return Result<RefreshToken>.GeneralFailure($"خطا در اعتبارسنجی Token: {ex.Message}");
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
                _logger.LogError(ex, "خطا در حذف Token های منقضی");
                return Result.GeneralFailure("خطا در حذف Token های منقضی");
            }
        }

        public async Task<Result> RevokeRefreshTokenByUserIdAsync(int userId)
        {
            try
            {
                var refreshTokens = await _refreshTokenRepository
                    .GetDbSet()
                    .Where(r => r.UserAccountId == userId)
                    .ToListAsync();

                if (!refreshTokens?.Any() ?? true)
                {
                    return Result.NotFoundFailure("هیچ RefreshToken فعالی برای این کاربر یافت نشد");
                }

                foreach (var token in refreshTokens)
                {
                    token.IsRevoked = true;
                    var updateResult = UpdateRefreshToken(token);

                    if (updateResult.IsFailure)
                    {
                        _logger.LogWarning($"خطا در لغو RefreshToken با ID: {token.Id}");
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در لغو RefreshToken های کاربر {userId}");
                return Result.GeneralFailure("خطا در لغو RefreshToken های کاربر");
            }
        }

        /// <summary>
        /// نمونه‌ای از استفاده از Extension Methods برای تبدیل به ServiceResult
        /// این متد نشان می‌دهد چگونه می‌توان سازگاری با کد موجود را حفظ کرد
        /// </summary>
        public ServiceResult AddRefreshTokenLegacy(RefreshToken refreshToken)
        {
            var result = AddRefreshToken(refreshToken);
            return result.ToServiceResult();
        }
    }
}