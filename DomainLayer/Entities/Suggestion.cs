using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Suggestion : BaseEntityModel, IAuditableEntity
{
    public int RequestId { get; set; }

    public int UserAccountId { get; set; }

    public decimal SuggestionPrice { get; set; }

    public int Currency { get; set; }

    public int ItemType { get; set; }

    public string Description { get; set; }

    public int Status { get; set; }

    public UserAccount UserAccount { get; set; }

    public Request Request { get; set; }

    public ICollection<RequestStatusHistory> RequestStatusHistories { get; set; } = [];

    public ICollection<SuggestionAttachment> SuggestionAttachments { get; set; } = [];
}