using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class WalletTransaction : BaseEntityModel, IAuditableEntity
{
    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public int TransactionType { get; set; }

    public string RelatedEntity { get; set; }

    public decimal BalanceAfter { get; set; }

    public Wallet Wallet { get; set; }
}