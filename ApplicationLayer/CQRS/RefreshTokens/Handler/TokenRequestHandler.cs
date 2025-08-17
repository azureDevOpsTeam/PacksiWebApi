using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.RefreshTokens.Query;
using ApplicationLayer.DTOs;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.RefreshTokens.Handler;

public class TokenRequestHandler(IIdentityService identityService,
                                      IRefreshTokenService refreshTokenService,
                                      IUserAccountServices userAccountServices,
                                      IUnitOfWork unitOfWork,
                                      ILogger<TokenRequestDto> logger) : IRequestHandler<TokenRequestQuery, HandlerResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<TokenRequestDto> _logger = logger;

    public async Task<HandlerResult> Handle(TokenRequestQuery request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var result = await _refreshTokenService.VerifyTokenAsync(request.Model);

            if (result.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return result.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var storedToken = result.Value;

            var userModel = await _userAccountServices.GetUserAccountByIdAsync(storedToken.UserAccountId);

            var token = await _identityService.TokenRequestGeneratorAsync(userModel.UserName, userModel.Id);

            var refreshToken = _refreshTokenService.RefreshTokenGenerator(userModel.Id, token.tokenId);
            refreshToken.UserFullName = storedToken.UserFullName;

            var refreshTokenResult = _refreshTokenService.AddRefreshToken(refreshToken);

            if (refreshTokenResult.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return refreshTokenResult.ToHandlerResult();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();

            var authorizeResultViewModel = new AuthorizeResultDto()
            {
                AccessTokens = token.jwtToken,
                RefreshToken = refreshToken.Token,
                UserFullName = storedToken.UserFullName
            };

            return new HandlerResult
            {
                RequestStatus = RequestStatus.Successful,
                ObjectResult = authorizeResultViewModel,
                Message = CommonMessages.Successful
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(message: ex.Message, CommonMessages.Failed);
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                ObjectResult = request.Model,
                Message = CommonMessages.Failed
            };
        }
    }
}