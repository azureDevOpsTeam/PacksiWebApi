using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Comment : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public string Content { get; set; } = default!;

    public bool IsApproved { get; set; } = false;

    public UserAccount UserAccount { get; set; }
}