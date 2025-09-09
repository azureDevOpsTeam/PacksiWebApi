using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class ViolationReport : BaseEntityModel, IAuditableEntity
{
    public int UserAccountId { get; set; }

    public int RequestId { get; set; }

    public int CauseViolation { get; set; }

    public string Description { get; set; }
}
