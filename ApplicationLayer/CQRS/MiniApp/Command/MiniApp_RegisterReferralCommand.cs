using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_RegisterReferralCommand(long TelegramUserId, string ReferralCode) : IRequest<bool>;

public class MiniApp_RegisterReferralCommandHandler(IUnitOfWork unitOfWork, IUserAccountServices userAccountServices, IRepository<Referral> referralRepository) : IRequestHandler<MiniApp_RegisterReferralCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IRepository<Referral> _referralRepository = referralRepository;

    public async Task<bool> Handle(MiniApp_RegisterReferralCommand request, CancellationToken cancellationToken)
    {
        var exists = await _referralRepository.Query()
            .FirstOrDefaultAsync(r => r.InviteeTelegramUserId == request.TelegramUserId
                                     && r.ReferralCode == request.ReferralCode
                                     && r.Status == ReferralStatusEnum.Pending,
                                 cancellationToken);

        if (exists != null) return true;

        var getInviter = await _userAccountServices.GetUserAccountInviterAsync(request.ReferralCode);
        if (getInviter.IsSuccess)
        {
            var existInvite = await _userAccountServices.GetExistReferralAsync(request.TelegramUserId);
            if (existInvite.IsFailure)
            {
                var referral = new Referral
                {
                    InviterUserId = getInviter.Value.TelegramId,
                    InviteeTelegramUserId = request.TelegramUserId,
                    ReferralCode = request.ReferralCode,
                    Status = ReferralStatusEnum.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _referralRepository.AddAsync(referral);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return true;
    }
}