using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Identities.Query;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.CQRS.Identities.Handler
{
    public class SignInHandler(IIdentityService identityService,
                                   IRefreshTokenService refreshTokenService,
                                   IUserAccountServices userAccountServices,
                                   IUnitOfWork unitOfWork) : IRequestHandler<SignInQuery, HandlerResult>
    {
        private readonly IIdentityService _identityService = identityService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IUserAccountServices _userAccountServices = userAccountServices;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<HandlerResult> Handle(SignInQuery request, CancellationToken cancellationToken)
        {
            var userResult = _userAccountServices.GetUserByValidationMethodAsync(request.Model);

            var userModel = (UserAccount)userResult.Data;

            var serviceResult = new ServiceResult();

            if (userResult.RequestStatus == RequestStatus.Successful)
            {
                if (request.Model.ValidationMethod == ValidationMethodEnum.OneTimePasswordEmail || request.Model.ValidationMethod == ValidationMethodEnum.OneTimePasswordMobile)
                    serviceResult = _identityService.AuthenticateOneTimePassword(request.Model, (UserAccount)userResult.Data);
                else
                    serviceResult = await _identityService.AuthenticateUserInformationAsync(request.Model, userModel);
            }
            else
            {
                return new HandlerResult
                {
                    RequestStatus = userResult.RequestStatus,
                    Message = userResult.Message
                };
            }

            if (serviceResult.RequestStatus != RequestStatus.Successful)
            {
                return new HandlerResult
                {
                    RequestStatus = serviceResult.RequestStatus,
                    Message = serviceResult.Message
                };
            }

            var authorizeResult = (AuthorizeResultDto)serviceResult.Data;

            var refreshToken = _refreshTokenService.RefreshTokenGenerator(userModel.Id, authorizeResult.TokenId);
            refreshToken.UserFullName = authorizeResult.UserFullName;
            var refreshTokenResult = _refreshTokenService.AddRefreshToken(refreshToken);

            if (refreshTokenResult.RequestStatus != RequestStatus.Successful)
                return new HandlerResult
                {
                    RequestStatus = refreshTokenResult.RequestStatus,
                    Message = refreshTokenResult.Message
                };

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            authorizeResult.AccessTokens = authorizeResult.AccessTokens;
            authorizeResult.RefreshToken = refreshToken.Token;

            return new HandlerResult
            {
                RequestStatus = serviceResult.RequestStatus,
                ObjectResult = authorizeResult,
                Message = serviceResult.Message
            };
        }
    }
}