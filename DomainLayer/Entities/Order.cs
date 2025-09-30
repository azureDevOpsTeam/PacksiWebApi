using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Order : BaseEntityModel, IAuditableEntity
{
    public int SuggestionId { get; set; }

    public decimal Amount { get; set; }

    public int Currency { get; set; }

    public int Status { get; set; }

    public string PaymentTrackingCode { get; set; }

    public string WalletAddress { get; set; }

    public string Network { get; set; }

    public string TxId { get; set; }

    public Suggestion Suggestion { get; set; }
}