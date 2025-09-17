using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_RegisterReferralCommand(long TelegramUserId, string ReferralCode) : IRequest<bool>;

public class MiniApp_RegisterReferralCommandHandler(IUnitOfWork unitOfWork, IRepository<Referral> referralRepository) : IRequestHandler<MiniApp_RegisterReferralCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRepository<Referral> _referralRepository = referralRepository;

    public async Task<bool> Handle(MiniApp_RegisterReferralCommand req, CancellationToken cancellationToken)
    {
        var exists = await _referralRepository.Query()
            .FirstOrDefaultAsync(r => r.InviteeTelegramUserId == req.TelegramUserId
                                     && r.ReferralCode == req.ReferralCode
                                     && r.Status == ReferralStatusEnum.Pending,
                                 cancellationToken);

        if (exists != null) return true;

        var referral = new Referral
        {
            InviteeTelegramUserId = req.TelegramUserId,
            ReferralCode = req.ReferralCode,
            Status = ReferralStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _referralRepository.AddAsync(referral);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}