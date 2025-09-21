using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Wallet : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public int Currency { get; set; }

    public decimal Balance { get; set; }

    public decimal Reserved { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public UserAccount UserAccount { get; set; }

    public ICollection<WalletTransaction> WalletTransactions { get; set; } = [];
}