using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Query;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using DomainLayer.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class UserValicationHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices, IConfiguration configuration, IUserAccountServices userAccountServices, ILogger<UserValicationHandler> logger, IMapper mapper) : IRequestHandler<UserValicationQuery, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UserValicationHandler> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<HandlerResult> Handle(UserValicationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var validationResult = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
            if (validationResult.IsFailure)
            {
                await _unitOfWork.RollbackAsync();
                return validationResult.ToHandlerResult();
            }

            if (!validationResult.Value.ExistUser)
            {
                var inviter = await _userAccountServices.GetReferralAsync(validationResult.Value.User.Id);
                if (inviter.IsSuccess)
                    validationResult.Value.User.ReferredByUserId = inviter.Value.InviterUserId;

                var userAccont = await _userAccountServices.MiniApp_AddUserAccountAsync(validationResult.Value.User);
                if (userAccont.RequestStatus != RequestStatus.Successful)
                {
                    await _unitOfWork.RollbackAsync();
                    return new HandlerResult { RequestStatus = userAccont.RequestStatus, Message = userAccont.Message };
                }

                await _unitOfWork.SaveChangesAsync();

                var userProfile = _mapper.Map<UserProfile>(validationResult.Value.User);
                var userAccount = (UserAccount)userAccont.Data;
                userProfile.UserAccountId = userAccount.Id;
                var resultUserProfile = await _userAccountServices.MiniApp_AddProfileAsync(userProfile);
                if (resultUserProfile.RequestStatus != RequestStatus.Successful)
                {
                    await _unitOfWork.RollbackAsync();
                    return new HandlerResult { RequestStatus = resultUserProfile.RequestStatus, Message = resultUserProfile.Message };
                }
                await _unitOfWork.SaveChangesAsync();
            }

            var result = await _userAccountServices.MiniApp_RequiredOperationAsync();

            await _unitOfWork.CommitAsync();

            return new HandlerResult { RequestStatus = RequestStatus.Successful, ObjectResult = result.Data, Message = CommonMessages.Successful };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            await _unitOfWork.RollbackAsync();
            return new HandlerResult
            {
                RequestStatus = RequestStatus.Failed,
                Message = CommonMessages.Failed
            };
        }
    }
}