using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Identities.Command;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using DomainLayer.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.Identities.Handler
{
    public class SignUpHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IMapper mapper, ILogger<SignUpHandler> logger) : IRequestHandler<SignUpCommand, HandlerResult>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IUserAccountServices _userAccountServices = userAccountServices;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<SignUpHandler> _logger = logger;

        public async Task<HandlerResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existuser = await _userAccountServices.ExistUserAsync(request.Model);
                if (existuser)
                    return new HandlerResult { RequestStatus = RequestStatus.Exists, Message = CommonMessages.Exist };

                var tgRegister = await _userAccountServices.RegisterWithTelegramAsync(request.Model);
                await _unitOfWork.BeginTransactionAsync();

                if (tgRegister)
                {
                    var getuser = await _userAccountServices.GetUserAccountByPhoneNumberAsync(request.Model.PhoneNumber);
                    var result = await _userAccountServices.MergeToTelegramAccountAsync(getuser, request.Model.Password);
                    if (result.RequestStatus == RequestStatus.Successful)
                    {
                        var userAccount = (UserAccount)result.Data;
                        var userProfile = userAccount.UserProfiles.FirstOrDefault();
                        userProfile.DisplayName = request.Model.DisplayName;
                        await _userAccountServices.UpdateUserProfileAsync(userProfile);
                        var roleResult = await _userAccountServices.AssignRoleToUserAsync(getuser.Id, "User", cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        await _unitOfWork.CommitAsync();

                        return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
                    }
                    return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
                }
                else
                {
                    var userAccount = _mapper.Map<UserAccount>(request.Model);
                    var userProfile = _mapper.Map<UserProfile>(request.Model);

                    userAccount.Password = HashGenerator.GenerateSHA256HashWithSalt(request.Model.Password, out string securityStamp);
                    userAccount.SecurityStamp = securityStamp;

                    if (!string.IsNullOrWhiteSpace(request.Model.InviteCode))
                    {
                        var invitation = await _userAccountServices.GetValidInvitationAsync(request.Model.InviteCode, cancellationToken);

                        if (invitation.RequestStatus != RequestStatus.Successful)
                        {
                            await _unitOfWork.RollbackAsync();
                            return new HandlerResult
                            {
                                RequestStatus = RequestStatus.ValidationFailed,
                                Message = "کد دعوت معتبر نیست یا منقضی شده است."
                            };
                        }

                        var result = (Invitation)invitation.Data;
                        userAccount.InvitedByManagerId = result.InviterUserId;
                        result.UsageCount++;
                    }
                    var userAccountResult = await _userAccountServices.AddUserAccountAsync(userAccount);

                    if (userAccountResult.RequestStatus != RequestStatus.Successful)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new HandlerResult { RequestStatus = userAccountResult.RequestStatus, Message = userAccountResult.Message };
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    userProfile.UserAccountId = userAccount.Id;
                    var profileResult = await _userAccountServices.AddProfileAsync(userProfile);
                    var roleResult = await _userAccountServices.AssignRoleToUserAsync(userAccount.Id, "User", cancellationToken);

                    if (roleResult.RequestStatus != RequestStatus.Successful)
                    {
                        await _unitOfWork.RollbackAsync();
                        return new HandlerResult
                        {
                            RequestStatus = RequestStatus.Failed,
                            ObjectResult = request.Model,
                            Message = "ثبت نقش کاربر با خطا مواجه شد."
                        };
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitAsync();

                    return new HandlerResult { RequestStatus = userAccountResult.RequestStatus, Message = userAccountResult.Message };
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                await _unitOfWork.RollbackAsync();
                return new HandlerResult { RequestStatus = RequestStatus.Failed, Message = CommonMessages.Failed };
            }
        }
    }
}