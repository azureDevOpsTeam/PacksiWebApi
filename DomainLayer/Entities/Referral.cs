using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Referral : BaseEntityModel, IAuditableEntity
{
    public long InviteeTelegramUserId { get; set; }

    public string ReferralCode { get; set; } = default!;

    public int? InviterUserId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }
}