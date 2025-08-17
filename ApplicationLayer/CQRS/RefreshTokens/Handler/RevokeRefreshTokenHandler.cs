using ApplicationLayer.CQRS.RefreshTokens.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.RefreshTokens;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.RefreshTokens.Handler
{
    public class RevokeRefreshTokenHandler(IRefreshTokenService refreshTokenService,
                                                  IUnitOfWork unitOfWork,
                                                  ILogger<RevokeRefreshTokenDto> logger) : IRequestHandler<RevokeRefreshTokenCommand, HandlerResult>
    {
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<RevokeRefreshTokenDto> _logger = logger;

        public async Task<HandlerResult> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _refreshTokenService.RevokeRefreshTokenByUserId(request.Model.UserId);

                if (result.IsFailure)
                    return result.ToHandlerResult();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return result.ToHandlerResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در لغو توکن‌های کاربر");
                return Result.GeneralFailure("خطا در لغو توکن‌های کاربر").ToHandlerResult();
            }
        }
    }
}